using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility
{
   public static class NullableTypeDefaultValueExtend
    {
       public static int GetValue(this int? source,int defaultValue) {
           return source.HasValue ? source.Value : defaultValue;
       }

       public static decimal GetValue(this decimal? source, decimal defaultValue)
       {
           return source.HasValue ? source.Value : defaultValue;
       }

       public static float GetValue(this float? source, float defaultValue)
       {
           return source.HasValue ? source.Value : defaultValue;
       }
    }
}
