using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.ChesFrame.Web.Model
{
    [Serializable]
    public class PageInfo
    {
        public int? Index { get; set; }

        public int? Size { get; set; }

        public int? Count { get; set; }

        /// <summary>
        /// 支持多个列的升序 降序 无序(非升靡降) 排列，并以符号隔开（比如“,”）
        /// </summary>
        public string SortBy { get; set; }
    }

    [Serializable]
    public class QueryCondition
    {
        public PageInfo PageInfo { get; set; }
    }

    [Serializable]
    public class QueryResult<R>
    {
        public PageInfo PageInfo { get; set; }

        public List<R> Data { get; set; }
    }
}