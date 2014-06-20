using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace System.ChesFrame.Utility.WCF
{
    public class ConfigSetting
    {
    }

    public class ClientConfig
    {
        [XmlElement]
        public string AssemblyFilePath;
        [XmlElement]
        public Binding Binding;
        [XmlElement]
        public string BaseAddress;
        [XmlElement]
        List<IContractBehavior> ContractBehaviorList;
        [XmlElement]
        List<IOperationBehavior> OperationBehaviorList;
        [XmlElement]
        bool MetaDataPushable;
    }
}
