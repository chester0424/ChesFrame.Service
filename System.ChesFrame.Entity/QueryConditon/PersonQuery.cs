using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Entity.QueryConditon
{
    public class PersonQuery : QueryBase
    {

        public int? SysNo { get; set; }

        public string Name { get; set; }

        public int? Age { get; set; }

        public string Phone { get; set; }
    }
}
