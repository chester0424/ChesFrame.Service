using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Entity
{
    //[Serializable]
    [DataContract]
    public class PersonEntity
    {
        [DataMember]
        public int? SysNo { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int? Age { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public Sex? Sex { get; set; }
        [DataMember]
        public string Remark { get; set; }
    }
}
