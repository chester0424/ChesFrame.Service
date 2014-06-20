using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Entity.SystemManage
{
    /// <summary>
    /// 用户 实体
    /// </summary>
    public class UserEntity
    {
        public int? SysNo { get; set; }

        public string Name { get; set; }

        public string LoginName { get; set; }

        public string PassWord { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int? Sex { get; set; }

        public string Token { get; set; }

        public string Remark { get; set; }
    }
}
