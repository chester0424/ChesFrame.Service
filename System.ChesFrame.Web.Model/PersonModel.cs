using System;
using System.ChesFrame.Entity;
using System.ChesFrame.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Web.Model
{
    [Serializable]
    public class PersonModel
    {
        public int? SysNo { get; set; }

        public string Name { get; set; }

        public int? Age { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public Sex? Sex { get; set; }

        public IList<KeyValuePair<string, string>> SexList 
        {
            get 
            {
                return EmumHelper.GetEnumItemDescription<Sex>(SelectModel.SelectAll);
            }
        }

        public string Remark { get; set; }
    }
}
