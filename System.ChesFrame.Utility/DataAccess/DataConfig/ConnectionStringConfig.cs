using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace System.ChesFrame.Utility.DataAccess.DataConfig
{
    /// <summary>
    /// 连接字符串
    /// </summary>
    public class ConnectionStringItem
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("connectionString")]
        public string ConnectionString { get; set; }

        [XmlAttribute("encrypted")]
        public bool Encrypted { get; set; }
        [XmlAttribute("providerName")]
        public string DbProviderName { get; set; }
    }

    /// <summary>
    /// 连接字符串集
    /// </summary>
    public class ConnectionStrings
    {
        [XmlElement("add")]
        public List<ConnectionStringItem> ConnectionString { get; set; }
    }

    /// <summary>
    /// 配置 序列化 根元素
    /// </summary>
    [XmlRoot("configuration")]
    public class ConnectionStringConfig
    {
        [XmlElement("connectionStrings")]
        public ConnectionStrings ConnectionStrings { get; set; }
    }
}
