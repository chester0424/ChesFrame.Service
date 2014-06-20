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
    public class DBCommandFilesConfig
    {
        [XmlArray("commandFiles"), XmlArrayItem("commandFile")]
        public List<CommandFile> CommandFiles { get; set; }
    }

    public class CommandFile
    {
        [XmlAttribute("filePath")]
        public string FilePath { get; set; }
    }
}
