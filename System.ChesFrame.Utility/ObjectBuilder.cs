using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.ObjectBuilder
{
    /// <summary>
    /// 对象创建
    /// </summary>
    public static  class ObjectBuilder
    {
        /// <summary>
        /// 表达式 创建 对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateObjByExpression<T>()
        {
            var newExpression = Expression.New(typeof(T));
            return Expression.Lambda<Func<T>>(newExpression, null).Compile()();
        }

        /// <summary>
        /// Emit 创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateObjByEmit<T>()
        {
            Type type = typeof(T);
            var ctor = type.GetConstructors()[0];
            DynamicMethod method = new DynamicMethod(String.Empty, typeof(T), null);
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ret);
            return (method.CreateDelegate(typeof(Func<T>)) as Func<T>)();
        }

        /// <summary>
        /// Activator 创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateObjByActivator<T>()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        /// <summary>
        /// Generic Activator 创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateObjByActivetorGeneric<T>()
        {
            return (T)Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Relection 执行构造函数 创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateObjByRelec<T>()
        {
            var method = typeof(T).GetConstructors()[0];
           return (T)method.Invoke(null);
        }
    }
}
