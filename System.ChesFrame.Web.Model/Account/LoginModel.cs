using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Web.Model.Account
{
    public class LoginModel
    {
        public string UserName { get; set; }

        public string PassWord { get; set; }

        public string VerificationCode { get; set; }

        public bool?  RememberMe { get; set; }
    }
}
