using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility
{
    public static class ConvertExtend
    {
        public static string EmptyToNull(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }
            else { return source; }
        }
    }
}
