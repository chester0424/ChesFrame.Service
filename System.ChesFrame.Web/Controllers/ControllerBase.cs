using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace System.ChesFrame.Web.Controllers
{
    public class ControllerBase:Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            
        }
    }
}