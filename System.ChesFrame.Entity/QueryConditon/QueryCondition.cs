using System;
using System.ChesFrame.Utility.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Entity.QueryConditon
{
    public class QueryConditionBase
    {
        public PageInfo PageInfo { get; set; }
    }

    //项目中定义的分页信息类
    public class PageInfo
    {
        public int? Index { get; set; }

        public int? Size { get; set; }

        public int? Count { get; set; }

        /// <summary>
        /// 支持多个列的升序 降序 无序(非升靡降) 排列，并以符号隔开（比如“,”）
        /// </summary>
        public string SortBy { get; set; }

        //对PageInfo进行自动数据类型转换成Pagination，方便SQL脚本构造
        public static implicit operator Pagination(PageInfo pageInfo) {
            return new Pagination()
            {
                PageIndex = pageInfo.Index ?? 0,
                PageSze = pageInfo.Size ?? 10,
                OrderBy = pageInfo.SortBy
            };
        }
    }
}
