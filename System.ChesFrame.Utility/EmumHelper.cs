using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility
{
   public static class EmumHelper
   {
       #region 获取枚举描述

       /// <summary>
       /// 获取枚举项值和描述
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <returns></returns>
       public static IList<KeyValuePair<string, string>> GetEnumItemDescription<T>(SelectModel selectModel = SelectModel.None)
       {
           var type = typeof(T);
           if (!type.IsEnum)
           {
               throw new InvalidOperationException("错误的枚举类型");
           }
           
           IList<KeyValuePair<string, string>> keyValuePair = new List<KeyValuePair<string, string>>();

           switch(selectModel)
           {
               case SelectModel.SelectAll:
               keyValuePair.Add(new KeyValuePair<string, string>("0", "--全选--"));
               break;
               case SelectModel.SelectPlease:
               keyValuePair.Add(new KeyValuePair<string, string>("", "--请选择--"));
               break;
           }

           FieldInfo[] fields = type.GetFields();
           foreach (var item in fields)
           {
               if (item.FieldType.IsEnum == false) { continue; }
               string desription = string.Empty;
               object[] objs = item.GetCustomAttributes(typeof(DescriptionAttribute), false);
               if (objs != null && objs.Length > 0)
               {
                   DescriptionAttribute da = (DescriptionAttribute)objs[0];
                   desription = da.Description;
               }
               else
               {
                   desription = item.Name;
               }
               keyValuePair.Add(new KeyValuePair<string, string>(((int)Enum.Parse(type, item.Name)).ToString(), desription));
           }
           return keyValuePair;
       }

       /// <summary>
       /// 获取枚举描述
       /// </summary>
       /// <param name="value"></param>
       /// <returns></returns>
       public static string EnumDescription(this Enum value)
       {
           FieldInfo fi = value.GetType().GetField(value.ToString());
           DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
           if (attributes.Length > 0)
           {
               return attributes[0].Description;
           }
           else
           {
               return value.ToString();
           }
       }

       /// <summary>
       /// 获取枚举描述
       /// </summary>
       /// <param name="value"></param>
       /// <returns></returns>
       public static string GetEnumDescription(Enum value)
       {
           return value.EnumDescription();
       }

       /// <summary>
       /// 获取枚举描述
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="value"></param>
       /// <returns></returns>
       public static string GetEnumDescription<T>(string value)
       {
           FieldInfo fi = typeof(T).GetField(value);
           DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
           if (attributes.Length > 0)
           {
               return attributes[0].Description;
           }
           else
           {
               return value.ToString();
           }
       }

       /// <summary>
       /// 获取枚举描述
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="value"></param>
       /// <returns></returns>
       public static string GetEnumDescription<T>(int value) where T : struct
       {
           var type = typeof(T);
           if (!type.IsEnum)
           {
               throw new InvalidOperationException("错误的枚举类型");
           }
           T t;

           if (Enum.TryParse(value.ToString(), out t))
           {
               return GetEnumDescription<T>(t.ToString());
           }
           return value.ToString();
       }

       #endregion

   }

   public enum SelectModel
   {
       None,
       SelectPlease,
       SelectAll
   }

    
}
