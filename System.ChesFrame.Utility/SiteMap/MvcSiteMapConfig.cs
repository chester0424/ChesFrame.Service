using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace System.ChesFrame.Utility.SiteMap
{
    [XmlRoot("mvcSiteMap")]
    public class MvcSiteMapConfig
    {
        [XmlElement("mvcSiteMapNode")]
        public List<MvcSiteMapNode> MvcSiteMapNodes { get; set; } 
    }

    public class MvcSiteMapNode 
    {
        [XmlAttribute("title")]
        public string Title{get;set;}

        [XmlAttribute("controller")]
        public string Controller{get;set;}

        [XmlAttribute("action")]
        public string Action{get;set;}

        [XmlElement("mvcSiteMapNode")]
        public List<MvcSiteMapNode> MvcSiteMapNodes{get;set;} 
    }
}
