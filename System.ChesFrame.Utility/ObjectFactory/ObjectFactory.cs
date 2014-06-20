using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.ObjectFactory
{
    public class ObjectFactory
    {
        public static T Create<T>()
        {
            return MappingConfigContainner.Instance.GetTypeByAbstract<T>();
        }
    }
}
