using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace System.ChesFrame.Utility.WCF.Config
{
    [XmlRoot("configuration")]
    public class ServiceAssemblyConfig
    {
        [XmlElement("add")]
        public List<ServiceAssemblyConfigItem> Items { get; set; }
    }

    public class ServiceAssemblyConfigItem
    {
        [XmlAttribute("dllName")]
        public string DllName { get; set; }
    }
}
