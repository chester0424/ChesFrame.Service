using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Entity.SystemManage
{
    /// <summary>
    /// 用户与权限关联表
    /// </summary>
    public class UserLimiteEntity
    {
        public int? SysNo { get; set; }

        public int? UserSysNo { get; set; }

        public int? LimiteSysNo { get; set; }

        public string Remark { get; set; }
    }
}
