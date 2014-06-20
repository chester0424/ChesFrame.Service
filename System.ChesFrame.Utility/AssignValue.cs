using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility
{
    /// <summary>
    /// 对象间赋值
    /// </summary>
    public static class AssignValue
    {
        /// <summary>
        /// 把当前对象的值赋值给新的类型（对象）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T AssignValueTo<T>(this object source) where T:new()
        {
            T t = new T();
            t.AssignValueFrom(source);
            return t;
        }

        public static List<T> AssignValueToList<S, T>(this List<S> source) where T : new()
        {
            List<T> list = new List<T>();
            foreach (var sub in source)
            {
                list.Add(sub.AssignValueTo<T>());
            }
            return list;
        }

        /// <summary>
        /// 从另外的对象中获取属性值赋值给当前对象
        /// </summary>
        /// <param name="starget"></param>
        /// <param name="source"></param>
        public static object AssignValueFrom(this object starget, object source)
        {
            if (starget == null || source == null)
            {
                var errorMsg = string.Format("{0} 和 {1} 不能为空", starget.ToString(), source.ToString());
                throw new ArgumentNullException("src");
            }

            PropertyInfo[] srcProperties = source.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            PropertyInfo[] dstProperties = starget.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            foreach (var dstP in dstProperties)
            {
                var srcP = srcProperties.FirstOrDefault(p => p.Name.Equals(dstP.Name));
                if (srcP == null) continue;

                dstP.SetValue(starget, HackType(srcP.GetValue(source), dstP.PropertyType), null);
                //if (dstP.PropertyType.IsGenericType)
                //{ continue; }

                //dstP.SetValue(starget, srcP.GetValue(source),null);
            }


            return starget;

          

            /*
             * 1.判断简单类型
             * 2.判断是否是复杂类型
             *      2.1复杂类型是否是集合类型
             */


            /*
             * c#中的数据类型
             * 值类型：
             * 引用类型：
             *      
             * 
             * 值类型：整数，浮点数，高精度浮点数，布尔，字符，结构，枚举
                引用类型：对象（Object），字符串，类，接口，委托，数组

                除了值类型和引用类型，还有一种数据类型是空类型（null）gv

                整数，浮点数，高精度浮点数，布尔，字符，对象（Object），字符串 是预定义的类型
                结构，枚举，类，接口，委托，数组 是复合类型
             * 
             * 枚举
             */

        }

        private static object HackType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))//如果是可空类型找其基础类型
            {
                if (value == null)
                    return null;

                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            if (typeof(System.Enum).IsAssignableFrom(conversionType))//枚举采用枚举转换 Enum.Parse方式
            {
                return Enum.Parse(conversionType, value.ToString());
            }
            return Convert.ChangeType(value, conversionType);//基本数据类型转化
        }

        #region 借助于AutoMapper实现对象间的赋值

        public static T AssignValueToByAutoMapper<S, T>(this S source) 
            where S : class,new()
            where T : class,new()
        {  
            return AutoMapper.MapTo<S, T>(source);
        }

        public static List<T> AssignValueToByAutoMapper<S, T>(this List<S> source)
            where S : class,new()
            where T : class,new()
        {

            return AutoMapper.MapTo<S, T>(source).ToList();
        }



        #endregion
    }
}
