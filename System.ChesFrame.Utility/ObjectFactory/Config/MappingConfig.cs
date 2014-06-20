using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace System.ChesFrame.Utility.ObjectFactory.Config
{
    [XmlRoot("configuration")]
    public class MappingConfig
    {
        [XmlElement("add")]
        public List<MappingConfigItem> Items { get; set; }

    }
    public class MappingConfigItem
    {
        [XmlAttribute("abstract")]
        public string Abstract { get; set; }

        [XmlAttribute("implement")]
        public string Implement { get; set; }
    }
}
