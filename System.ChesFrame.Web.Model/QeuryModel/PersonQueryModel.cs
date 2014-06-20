using System;
using System.ChesFrame.Web.Model;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.ChesFrame.Web.Model
{
    public class PersonQueryModel : QueryCondition
    {

        public int? SysNo { get; set; }

        public string Name { get; set; }

        public int? Age { get; set; }

        public string Phone { get; set; }
    }
}