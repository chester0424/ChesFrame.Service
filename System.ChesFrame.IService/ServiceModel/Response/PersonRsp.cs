using System;
using System.ChesFrame.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.IService.ServiceModel
{
    public class PersonRsp
    {

        public List<PersonEntity> Person { get; set; }

        public string OthIfo { get; set; }
    }
}
