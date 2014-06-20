using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility
{
    internal static class ReflectionUtils
    {
        //在运行时得到一个实现了IEnumerable<T>类型的泛型参数的类型。
        public static Type GetElementTypeOfIEnumerable(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (type.IsInterface)
            {
                Type def = type.GetGenericTypeDefinition();
                if (def == typeof(IEnumerable<>))
                {
                    return type.GetGenericArguments()[0];
                }
            }
            else
            {
                Type interfaceType = type
                    .GetInterfaces()
                    .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                if (interfaceType != null)
                {
                    return interfaceType.GetGenericArguments()[0];
                }
            }

            return null;
        }

        //判断一个类型是引用类型但不是string类型。
        public static bool IsReferenceTypeButString(Type type)
        {
            return !type.IsValueType && type != typeof(string);
        }
        //判断一个类型是值类型或者是string类型。
        public static bool IsValueTypeOrString(Type type)
        {
            return type.IsValueType || type == typeof(string);
        }
        //创建一个集合，如果是数组，就会使用count参数创建指定长度的数组
        public static IEnumerable<object> CreateCollection(Type collType, int count)
        {
            object instance = null;
            var elementType = ReflectionUtils.GetElementTypeOfIEnumerable(collType);

            if (collType.IsArray)
            {
                instance = Array.CreateInstance(elementType, count);
            }
            else if (collType.IsInterface)
            {
                var listType = typeof(List<>).MakeGenericType(elementType);

                if (collType.IsAssignableFrom(listType))
                {
                    instance = Activator.CreateInstance(listType);
                }
            }
            else
            {
                instance = CreateInstance(collType);
            }

            return instance as IEnumerable<object>;
        }
        //将sources中的对象拷贝到目标集合中。Target必须是一个数组或者实现了Ilist接口。
        public static void CopyElements(object[] source, object target)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            Array targetArray = target as Array;
            if (targetArray != null)
            {
                Array.Copy(source, targetArray, targetArray.Length);
            }
            else if (target is IList)
            {
                IList targetList = target as IList;
                foreach (var item in source)
                {
                    targetList.Add(item);
                }
            }
        }
        //为了递归进行映射，需要用反射创建对象实例，如果不能创建，那么映射过程将会失败。
        public static object CreateInstance(Type type)
        {
            try
            {
                return Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                throw new MappingException("Fail to do mapping,can't create instance for type: " + type, e);
            }
        }
    }

    //下面的类使用了开放委托，以非常高的性能获取和设置指定属性的值。
    internal class PropertyInvoker
    {
        public PropertyInvoker(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            //枚举类型将会按照枚举类型的基类型进行设置。本质上枚举类型是一个整数如int，short，byte等等。
            Property = property;
            Type propType = Property.PropertyType.IsEnum ?
               Property.PropertyType.GetEnumUnderlyingType() : Property.PropertyType;

            //Get public getter method.
            var getMethod = property.GetGetMethod();
            if (getMethod != null)
            {
                Type delType = typeof(Func<,>).MakeGenericType(Property.DeclaringType, propType);
                Getter = Delegate.CreateDelegate(delType, null, getMethod);
            }

            //Get public setter method
            var setMethod = property.GetSetMethod();
            if (setMethod != null)
            {
                Type delType = typeof(Action<,>).MakeGenericType(Property.DeclaringType, propType);
                Setter = Delegate.CreateDelegate(delType, null, setMethod);
            }
        }

        public PropertyInfo Property { get; private set; }
        public Delegate Getter { get; private set; }
        public Delegate Setter { get; private set; }
    }

    //下面的提供了从一个类型上得到所有PropertyInvoker实例的功能。实例被存放到字典中，并使用属性名索引。
    internal class PropertyInvokerBuilder
    {
        private static ConcurrentDictionary<Type, PropertyInvokerBuilder> _builders;
        private static Func<Type, PropertyInvokerBuilder> _createBuilder;

        static PropertyInvokerBuilder()
        {
            _builders = new ConcurrentDictionary<Type, PropertyInvokerBuilder>();
            _createBuilder = t => new PropertyInvokerBuilder(t);
        }

        /// <summary>
        /// Get PropertyInvokerBuilder for specified type,PropertyInvokerBuilder for same type is cached.
        /// </summary>
        public static PropertyInvokerBuilder Get(Type owner)
        {
            return _builders.GetOrAdd(owner, _createBuilder);
        }

        public Type OwnerType { get; private set; }
        public Dictionary<string, PropertyInvoker> Invokers { get; private set; }

        private PropertyInvokerBuilder(Type owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            OwnerType = owner;

            Invokers = OwnerType
              .GetProperties(BindingFlags.Public | BindingFlags.Instance)
              .ToDictionary(prop => prop.Name, prop => new PropertyInvoker(prop));
        }
    }
    //下面的接口限定了所有属性映射的需要实现的行为。就是将源对象的属性值映射到目标对象的属性内。
    internal interface IPropertyMap
    {
        void MapValue<TSource, TTarget>(TSource source, TTarget target)
            where TSource : class
            where TTarget : class;
    }

    //下面的类作为所有属性映射的基类，共享了一些实现。
    internal abstract class PropertyMap : IPropertyMap
    {
        protected static MethodInfo _mapGenericValue;

        static PropertyMap()
        {
            //为构建开放委托准备方法对象。
            _mapGenericValue = typeof(PropertyMap).GetMethod("MapGenericValue", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        //用于获取或者设置属性值的PropertyInvoker对象。
        protected PropertyInvoker _sourceInvoker;
        protected PropertyInvoker _targetInvoker;

        public PropertyMap(PropertyInvoker sourceInvoker, PropertyInvoker targetInvoker)
        {
            _sourceInvoker = sourceInvoker;
            _targetInvoker = targetInvoker;
        }

        //派生类必须实现如何来映射属性值。
        public abstract void MapValue<TSource, TTarget>(TSource source, TTarget target)
            where TSource : class
            where TTarget : class;
        //按照泛型方式来获取和设置属性值将会大大提高性能。       
        protected void MapGenericValue<TSource, TTarget, TValue>(TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            TValue v = ((Func<TSource, TValue>)_sourceInvoker.Getter)(source);
            ((Action<TTarget, TValue>)_targetInvoker.Setter)(target, v);
        }
    }

    //下面的类用于实现基本属性映射。运行时将会直接调用开放委托。性能非常不错。
    internal class BasicPropertyMap : PropertyMap
    {
        private Delegate _mapDelegate;

        public BasicPropertyMap(PropertyInvoker sourceInvoker, PropertyInvoker targetInvoker, Type propType)
            : base(sourceInvoker, targetInvoker)
        {
            Type sourceOwnerType = _sourceInvoker.Property.DeclaringType;
            Type targetOwnerType = _targetInvoker.Property.DeclaringType;
            var method = _mapGenericValue.MakeGenericMethod(sourceOwnerType, targetOwnerType, propType);
            var delType = typeof(Action<,,>).MakeGenericType(typeof(PropertyMap), sourceOwnerType, targetOwnerType);
            _mapDelegate = Delegate.CreateDelegate(delType, null, method);
        }

        public override void MapValue<TSource, TTarget>(TSource source, TTarget target)
        {
            ((Action<PropertyMap, TSource, TTarget>)_mapDelegate)(this, source, target);
        }
    }

    //下面的类用于实现两个不同类（非集合类型）之间的映射。
    internal class TwinPropertyMap : PropertyMap
    {
        public TwinPropertyMap(PropertyInvoker sourceInvoker, PropertyInvoker targetInvoker)
            : base(sourceInvoker, targetInvoker)
        { }

        public override void MapValue<TSource, TTarget>(TSource source, TTarget target)
        {
            //使用了泛型参数的逆变，提高性能
            object sourcePropValue = ((Func<TSource, object>)(_sourceInvoker.Getter))(source);

            if (sourcePropValue == null)
            {
                return;
            }

            object targetPropValue = null;

            //Check if property value iscreated by target class
            if (_targetInvoker.Getter != null)
            {
                //使用了泛型参数的逆变，提高性能
                targetPropValue = ((Func<TTarget, object>)_targetInvoker.Getter)(target);
            }

            //Create property value iftarget class doesn't create it.
            if (targetPropValue == null && _targetInvoker.Setter != null)
            {
                targetPropValue = ReflectionUtils.CreateInstance(_targetInvoker.Property.PropertyType);
                //此处只能用后期绑定来调用。性能差。
                _targetInvoker.Setter.DynamicInvoke(target, targetPropValue);
            }

            if (targetPropValue != null)
            {
                //递归映射引用类型的属性值。
                AutoMapper.MapObject(sourcePropValue, targetPropValue);
            }
        }
    }

    //集合映射的基类型，提供共享实现。
    internal abstract class EnumerablePropertyMap : PropertyMap
    {
        protected Type _targetCollType;
        protected Type _targetElementType;
        protected Type _sourceElementType;

        public EnumerablePropertyMap(PropertyInvoker sourceInvoker, PropertyInvoker targetInvoker)
            : base(sourceInvoker, targetInvoker)
        {
            _sourceElementType = ReflectionUtils.GetElementTypeOfIEnumerable(sourceInvoker.Property.PropertyType);
            _targetCollType = targetInvoker.Property.PropertyType;
            _targetElementType = ReflectionUtils.GetElementTypeOfIEnumerable(_targetCollType);
        }

        public override void MapValue<TSource, TTarget>(TSource source, TTarget target)
        {
            var sourceValues = ((Func<TSource, IEnumerable<object>>)(_sourceInvoker.Getter))(source);

            if (sourceValues == null)
            {
                return;
            }

            var coll = sourceValues as ICollection;
            int count = coll != null ? coll.Count : sourceValues.Count();

            if (count == 0)
            {
                return;
            }

            IEnumerable<object> targetEnumerable = null;

            if (_targetInvoker.Getter != null)
            {
                targetEnumerable = ((Func<TTarget, IEnumerable<object>>)(_targetInvoker.Getter))(target);
            }

            if (targetEnumerable == null && _targetInvoker.Setter != null)
            {
                targetEnumerable = ReflectionUtils.CreateCollection(_targetCollType, count);
                _targetInvoker.Setter.DynamicInvoke(target, targetEnumerable);
            }

            if (targetEnumerable != null)
            {
                var targetValues = GetTargetValues(sourceValues);
                ReflectionUtils.CopyElements(targetValues, targetEnumerable);
            }
        }

        //对于指定源对象集合，得到转换后目标对象集合。
        protected abstract object[] GetTargetValues(IEnumerable<object> sourceValues);
    }

    //当两个集合的元素类型相同时，使用源集合的值作为目标集合的元素值。
    internal class BasicEnumerablePropertyMap : EnumerablePropertyMap
    {
        public BasicEnumerablePropertyMap(PropertyInvoker sourceInvoker, PropertyInvoker targetInvoker)
            : base(sourceInvoker, targetInvoker) { }

        protected override object[] GetTargetValues(IEnumerable<object> sourceValues)
        {
            return sourceValues.ToArray();
        }
    }

    //当两个集合元素类型不同，而且都是引用类型但不是string的时候，对集合的每一个元素转换，然后将其添加到目标集合。
    internal class TwinEnumerablePropertyMap : EnumerablePropertyMap
    {
        public TwinEnumerablePropertyMap(PropertyInvoker sourceInvoker, PropertyInvoker targetInvoker)
            : base(sourceInvoker, targetInvoker)
        { }

        protected override object[] GetTargetValues(IEnumerable<object> sourceValues)
        {
            return AutoMapper.MapCollection(_sourceElementType, _targetElementType, sourceValues).ToArray();
        }
    }

    //下面的类提供了在两个类型之间建立映射的功能。他创建了一个IpropertyMap的集合，并使用其做实际映射。
    internal static class TypeMap<TSource, TTarget>
        where TSource : class
        where TTarget : class
    {
        private static List<IPropertyMap> _propertyMaps;

        //映射的规则在此处检测，其中用到了映射配置，后面会提到。
        static TypeMap()
        {
            _propertyMaps = new List<IPropertyMap>();

            var sourcePropInvokers = PropertyInvokerBuilder.Get(typeof(TSource)).Invokers;
            var targetPropInvokers = PropertyInvokerBuilder.Get(typeof(TTarget)).Invokers;

            //Create PropertyMap objectif two properties have same name and same type.
            foreach (string propName in sourcePropInvokers.Keys)
            {
                PropertyInvoker sourcePropInvoker = sourcePropInvokers[propName];
                if (sourcePropInvoker.Getter == null)
                {
                    continue;
                }

                if (MapConfiguration<TSource, TTarget>.Singleton.IsIgnored(propName))
                {
                    continue;
                }

                string targetPropName;
                if (!MapConfiguration<TSource, TTarget>.Singleton.TryGetMappedProperty(propName, out targetPropName))
                {
                    targetPropName = propName;
                }

                PropertyInvoker targetPropInvoker = null;
                if (!targetPropInvokers.TryGetValue(targetPropName, out targetPropInvoker))
                {
                    continue;
                }


                var sourcePropType = sourcePropInvoker.Property.PropertyType;
                var targetPropType = targetPropInvoker.Property.PropertyType;

                if (ReflectionUtils.IsValueTypeOrString(sourcePropType)
                  && ReflectionUtils.IsValueTypeOrString(targetPropType))
                {
                    if (targetPropInvoker.Setter != null)
                    {
                        Type propType = null;
                        //Properties have sameproperty type are mappable.
                        if (sourcePropType == targetPropType)
                        {
                            propType = sourcePropType.IsEnum ?
                                propType = sourcePropType.GetEnumUnderlyingType() : sourcePropType;
                        }
                        else if (sourcePropType.IsEnum && targetPropType.IsEnum)
                        {
                            //if they don't have sameproperty type, they should be enum type and have same underlying type.
                            var sourceBaseType = sourcePropType.GetEnumUnderlyingType();
                            var targetBaseType = targetPropType.GetEnumUnderlyingType();

                            if (sourceBaseType == targetBaseType)
                            {
                                propType = sourceBaseType;
                            }
                        }

                        if (propType != null)
                        {
                            var map = new BasicPropertyMap(sourcePropInvoker, targetPropInvoker, propType);
                            _propertyMaps.Add(map);
                        }
                    }
                }
                else if (ReflectionUtils.IsReferenceTypeButString(sourcePropType)
                  && ReflectionUtils.IsReferenceTypeButString(targetPropType))
                {
                    bool isSourceEnumerable = typeof(IEnumerable<object>).IsAssignableFrom(sourcePropType);
                    bool isTargetEnumerable = typeof(IEnumerable<object>).IsAssignableFrom(targetPropType);

                    if (isSourceEnumerable && isTargetEnumerable)
                    {
                        var sourceElementType = ReflectionUtils.GetElementTypeOfIEnumerable(sourcePropType);
                        var targetElementType = ReflectionUtils.GetElementTypeOfIEnumerable(targetPropType);

                        if (sourceElementType == targetElementType)
                        {
                            var map = new BasicEnumerablePropertyMap(sourcePropInvoker, targetPropInvoker);
                            _propertyMaps.Add(map);
                        }
                        else if (ReflectionUtils.IsReferenceTypeButString(sourceElementType)
                            && ReflectionUtils.IsReferenceTypeButString(sourceElementType))
                        {
                            var map = new TwinEnumerablePropertyMap(sourcePropInvoker, targetPropInvoker);
                            _propertyMaps.Add(map);
                        }
                    }
                    else if (sourcePropType == targetPropType)
                    {
                        if (targetPropInvoker.Setter != null)
                        {
                            var map = new BasicPropertyMap(sourcePropInvoker, targetPropInvoker, sourcePropType);
                            _propertyMaps.Add(map);
                        }
                    }
                    else
                    {
                        var map = new TwinPropertyMap(sourcePropInvoker, targetPropInvoker);
                        _propertyMaps.Add(map);
                    }
                }
            }
        }

        public static void Map(TSource source, TTarget target)
        {
            foreach (PropertyMap propMap in _propertyMaps)
            {
                propMap.MapValue<TSource, TTarget>(source, target);
            }
        }
    }

    //作为映射配置的基类，为所有泛型映射配置类提供共享存储。
    public class MapConfiguration
    {
        //存储每个类型映射的忽略的属性
        private ConcurrentDictionary<TypeMapKey, HashSet<string>> _ignoredProps;
        //存储每个类型映射的不同属性名映射
        private ConcurrentDictionary<TypeMapKey, Dictionary<string, string>> _mappedProps;

        public MapConfiguration()
        {
            _ignoredProps = new ConcurrentDictionary<TypeMapKey, HashSet<string>>();
            _mappedProps = new ConcurrentDictionary<TypeMapKey, Dictionary<string, string>>();
        }

        internal void IgnoreProperty(TypeMapKey key, string propName)
        {
            var props = _ignoredProps.GetOrAdd(key, k => new HashSet<string>());

            if (!props.Add(propName))
            {
                throw new MappingException(propName + " has beenignored.");
            }
        }

        internal void MapProperty(TypeMapKey key, string sourceProp, string targetProp)
        {
            var propMapping = _mappedProps.GetOrAdd(key, k => new Dictionary<string, string>());

            if (propMapping.ContainsKey(sourceProp))
            {
                throw new MappingException(sourceProp + " has beenmapped.");
            }

            propMapping.Add(sourceProp, targetProp);
        }

        internal bool IsIgnored(TypeMapKey key, string propName)
        {
            HashSet<string> props = null;

            if (!_ignoredProps.TryGetValue(key, out props))
            {
                TypeMapKey key2 = new TypeMapKey(key.Target, key.Source);

                if (!_ignoredProps.TryGetValue(key2, out props))
                {
                    return false;
                }

                _ignoredProps.TryAdd(key, props);
            }

            return props.Contains(propName);
        }

        internal bool TryGetMappedProperty(TypeMapKey key, string sourceProp, out string targetProp)
        {
            Dictionary<string, string> propMappings = null;
            if (!_mappedProps.TryGetValue(key, out propMappings))
            {
                TypeMapKey key2 = new TypeMapKey(key.Target, key.Source);

                if (!_mappedProps.TryGetValue(key2, out propMappings))
                {
                    targetProp = null;
                    return false;
                }

                propMappings = propMappings.ToDictionary(pair => pair.Value, pair => pair.Key);
                _mappedProps.TryAdd(key, propMappings);
            }

            return propMappings.TryGetValue(sourceProp, out targetProp);
        }
    }

    //按照Fluent API的方式提供映射配置。通过此派生类可以得到此映射的配置，而不用指定TypeMapKey。
    public class MapConfiguration<TSource, TTarget> : MapConfiguration
        where TSource : class
        where TTarget : class
    {
        private static TypeMapKey _mapKey;
        public static MapConfiguration<TSource, TTarget> Singleton { get; private set; }

        static MapConfiguration()
        {
            _mapKey = new TypeMapKey(typeof(TSource), typeof(TTarget));
            Singleton = new MapConfiguration<TSource, TTarget>();
        }

        public MapConfiguration<TSource, TTarget> Ignore<TProperty>(Expression<Func<TSource, TProperty>> propExp)
        {
            PropertyInfo prop = GetPropertyFromExpression(propExp, "propExp");
            IgnoreProperty(_mapKey, prop.Name);
            return this;
        }

        public MapConfiguration<TSource, TTarget> MapName<TProperty>(
             Expression<Func<TSource, TProperty>> sourcePropExp,
             Expression<Func<TTarget, TProperty>> targetPropExp)
        {
            var sourceProp = GetPropertyFromExpression(sourcePropExp, "sourcePropExp");
            var targetProp = GetPropertyFromExpression(targetPropExp, "targetPropExp");

            MapProperty(_mapKey, sourceProp.Name, targetProp.Name);
            return this;
        }

        public bool IsIgnored(string propName)
        {
            return base.IsIgnored(_mapKey, propName);
        }

        public bool TryGetMappedProperty(string sourceProp, out string targetProp)
        {
            return base.TryGetMappedProperty(_mapKey, sourceProp, out targetProp);
        }

        private PropertyInfo GetPropertyFromExpression(LambdaExpression exp, string paramName)
        {
            if (exp == null)
            {
                throw new ArgumentNullException(paramName);
            }

            MemberExpression expMember = exp.Body as MemberExpression;

            if (expMember == null || !(expMember.Member is PropertyInfo))
            {
                throw new ArgumentException(paramName, string.Format("Cannot get propertyname from {0} ", paramName));
            }

            return expMember.Member as PropertyInfo;
        }
    }

    //TypeMapKey类非处简单，它唯一标识一个映射。当作为映射配置的key的时候，只要两个类型相同，不论谁是source或target类型，都会得到相同的配置。
    internal class TypeMapKey
    {
        public TypeMapKey(Type source, Type target)
        {
            Source = source;
            Target = target;
        }

        public Type Source;
        public Type Target;

        public override int GetHashCode()
        {
            return Source.GetHashCode() ^ Target.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(TypeMapKey))
            {
                return false;
            }

            TypeMapKey other = (TypeMapKey)obj;
            return Source == other.Source && Target == other.Target;
        }
    }


    //最后的AutoMapper类的实现就非常简单了。正如示例中那样，使用AutoMapper的方式非常简单。

    public static class AutoMapper
    {
        private static MethodInfo _openMapToMethod;
        private static MethodInfo _openCollMapToMethod;
        private static ConcurrentDictionary<TypeMapKey, MethodInfo> _mapToMethods;
        private static ConcurrentDictionary<TypeMapKey, MethodInfo> _collMapToMethods;
        private static Func<TypeMapKey, MethodInfo> _createMapToMethod;
        private static Func<TypeMapKey, MethodInfo> _createCollMapToMethod;

        static AutoMapper()
        {
            MethodInfo[] mapToMethods = typeof(AutoMapper).GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (MethodInfo method in mapToMethods)
            {
                if (method.Name == "MapTo")
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == 2)
                    {
                        _openMapToMethod = method;
                    }
                    else if (parameters.Length == 1)
                    {
                        if (parameters[0].ParameterType.IsGenericType)
                        {
                            _openCollMapToMethod = method;
                        }
                    }
                }
            }

            _mapToMethods = new ConcurrentDictionary<TypeMapKey, MethodInfo>();
            _collMapToMethods = new ConcurrentDictionary<TypeMapKey, MethodInfo>();
            _createMapToMethod = key => _openMapToMethod.MakeGenericMethod(key.Source, key.Target);
            _createCollMapToMethod = key => _openCollMapToMethod.MakeGenericMethod(key.Source, key.Target);
        }

        public static MapConfiguration<TSource, TTarget> Configure<TSource, TTarget>()
            where TSource : class
            where TTarget : class
        {
            return MapConfiguration<TSource, TTarget>.Singleton;
        }

        /// <summary>
        /// Convert source items to target items.
        /// Empty collection will be returned if source itemcollection is null.
        /// </summary>
        public static IEnumerable<TTarget> MapTo<TSource, TTarget>(this IEnumerable<TSource> sourceItems)
            where TSource : class
            where TTarget : class, new()
        {
            if (sourceItems == null)
            {
                yield break;
            }

            foreach (TSource source in sourceItems)
            {
                yield return source.MapTo<TSource, TTarget>();
            }
        }

        /// <summary>
        /// Map source to target with specified type.
        /// </summary>   
        public static TTarget MapTo<TSource, TTarget>(this TSource source)
            where TSource : class
            where TTarget : class, new()
        {
            return source.MapTo<TSource, TTarget>(new TTarget());
        }

        /// <summary>
        /// Map source to target with specified type.
        /// Target object will be created if it is null.
        /// </summary>
        public static TTarget MapTo<TSource, TTarget>(this TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            if (source == null)
            {
                return null;
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            TypeMap<TSource, TTarget>.Map(source, target);
            return target;
        }

        internal static IEnumerable<object> MapCollection(Type sourceElementType, Type targetElementType, IEnumerable<object> sourceColl)
        {
            var key = new TypeMapKey(sourceElementType, targetElementType);
            var mapTo = _collMapToMethods.GetOrAdd(key, _createCollMapToMethod);
            var mappedObjs = (IEnumerable<object>)mapTo.Invoke(null, new[] { sourceColl });
            return mappedObjs;
        }

        /// <summary>
        /// Support for recursive map
        /// </summary>
        internal static void MapObject(object source, object target)
        {
            var key = new TypeMapKey(source.GetType(), target.GetType());
            var mapTo = _mapToMethods.GetOrAdd(key, _createMapToMethod);
            mapTo.Invoke(null, new[] { source, target });
        }
    }


    public class MappingException : Exception
    {
        public MappingException() { }

        public MappingException(string msg):base(msg) {
            
        }

        public MappingException(string msg, Exception ex):base(msg,ex) { }
    }

    /*
     * From URL http://blog.csdn.net/gentle_wolf/article/details/24417771
     */
}
