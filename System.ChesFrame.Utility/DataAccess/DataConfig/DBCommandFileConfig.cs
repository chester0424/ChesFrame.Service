using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace System.ChesFrame.Utility.DataAccess.DataConfig
{
    [XmlRoot("configuration")]
    public class DBCommandFileConfig
    {
        [XmlElement("dbCommand")]
        public List<DBCommand> DBCommands { get; set; }

    }

    public class DBCommand
    {
      
        [XmlIgnore]
        public int? CommandTimeout { get; set; }

        [XmlAttribute("commandTimeout")]
        public string CommandTimeoutValue {
            get {
                return CommandTimeout.HasValue ? CommandTimeout.Value.ToString() : null;
            }
            set {
                int result;
                CommandTimeout = int.TryParse(value, out result) ? result : (int?)null; 

            }
        }

        [XmlAttribute("commandType")]
        public string CommandType { get; set; }

        [XmlAttribute("server")]
        public string ServerName { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("commandText")]
        public string CommandText { get; set; }

        [XmlArray("parameters"), XmlArrayItem("parameter")]
        public List<DBParameter> Parameters { get; set; }
    }
    [Serializable]
    public class DBParameter
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("dbType")]
        public string DbType { get; set; }
        [XmlAttribute("size")]
        public int Size { get; set; }
        [XmlAttribute("direction")]
        public string Direction { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }
    }
}
