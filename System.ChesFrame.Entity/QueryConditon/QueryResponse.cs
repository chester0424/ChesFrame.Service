using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Entity.QueryConditon
{
    public class QueryResponse<T>
    {
        public PageInfo PageInfo { get; set; }

        public List<T> DataList { get; set; }

        public T Data { get; set; }
    }
}
