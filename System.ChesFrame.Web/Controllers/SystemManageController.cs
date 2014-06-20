using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace System.ChesFrame.Web.Controllers
{
    public class SystemManageController : Controller
    {
        #region 菜单

        public ActionResult Menu()
        {
            return View();
        }
        public ActionResult MenuQuery()
        {
            return View();
        }
        
        public ActionResult MenuAdd()
        {
            return View();
        }

        #endregion
    }
}