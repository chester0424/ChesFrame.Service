using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace System.ChesFrame.Utility.DataAccess.DataConfig
{
    public class SerializeHelper
    {
        public static T XmlDeserialize<T>(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlReader reader = XmlReader.Create(path);
            var result = (T)serializer.Deserialize(reader);
            return result;
        }

        public static void XmlSerialize<T>(T dataSource, string path)
        {

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;

            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                serializer.Serialize(writer, dataSource, ns);
            }
        }
    }
}
