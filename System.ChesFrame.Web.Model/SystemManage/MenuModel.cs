using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Web.Model.SystemManage
{
    public class MenuModel
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 相对ID
        /// </summary>
        public string RelationID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 打开文档方式
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// 显示排序优先级
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        public List<MenuModel> SubMenu { get; set; }
    }
}
