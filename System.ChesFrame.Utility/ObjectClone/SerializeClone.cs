using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.DataAccess
{
    public static class SerializeClone
    {
        /// <summary>
        /// 通过序列化的方式深拷贝
        /// </summary>
        /// <typeparam name="T">需要复制的对象类型</typeparam>
        /// <param name="obj">需要复制的对象</param>
        /// <returns>新的对象副本</returns>
        public static T DeepCloneBySerialize<T>(this object obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, obj);
            memoryStream.Position = 0;
            var t = (T)formatter.Deserialize(memoryStream);
            return t;
        }
    }
}
