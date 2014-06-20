using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.DataAccess
{
    /// <summary>
    /// 分页信息（DataAccess定义的分页类）
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// 页索引
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSze { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int AllCount { get; set; }

        /// <summary>
        /// 排序（排序类型+排序字段）
        /// </summary>
        public string OrderBy
        {
            //get
            //{
            //    return string.Format(" {0}{1}{2} ", OrderType.ToString(), " ", OrderByFild);
            //}
            //set
            //{
            //    var values = value.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            //    if (values != null && values.Count() >= 2)
            //    {
            //        OrderType orderType = DataAccess.OrderType.Asc;
            //        if (Enum.TryParse(values[0], out orderType))
            //        {
            //            OrderType = orderType;

            //            OrderByFild = values[1];
            //        }
            //    }
            //}
            get;
            set;
        }

        /// <summary>
        /// 排序类型
        /// </summary>
        public OrderType OrderType { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderByFild { get; set; }
    }
    public enum OrderType
    {
        Asc,
        Desc
    }

    //public class Pagination
    //{
    //    public int PageIndex { get; set; }

    //    /// <summary>
    //    /// 页大小
    //    /// </summary>
    //    public int PageSze { get; set; }

    //    /// <summary>
    //    /// 总数
    //    /// </summary>
    //    public int AllCount { get; set; }

    //    /// <summary>
    //    /// 排序（排序类型+排序字段）
    //    /// </summary>
    //    public string OrderBy
    //    {
    //        get;
    //        set;
    //    }
    //}

    //对象默认转PageInfoDic

    //public  class PageInfoExtend
    //{
    //    public static implicit operator Pagination(object obj)
    //    {
    //        var result = new Pagination();
    //        return (Pagination)result.AssignValueFrom(obj);
    //    }
    //}
}
