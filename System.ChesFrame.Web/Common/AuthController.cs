using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace System.ChesFrame.Web.Common
{
    public class AuthController:Controller
    {
        protected override void OnAuthentication(System.Web.Mvc.Filters.AuthenticationContext filterContext)
        {
            //base.OnAuthentication(filterContext);
            var value = filterContext.RequestContext.HttpContext.Request.Cookies["userName"];
            if (value == null)
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
            }

        }
    }
}