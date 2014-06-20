using System;
using System.ChesFrame.Web.Model.Account;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace System.ChesFrame.Web.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
            /*
             * 1.判断 帐号
             *  1.成功 通过 转向主页
             *  2.失败则返回login页面和错误信息
             */

            var checkResult  = false;
            if (!string.IsNullOrEmpty(loginModel.UserName)
                && !string.IsNullOrEmpty(loginModel.PassWord)) 
            {
                checkResult = true;
                HttpContext.Response.Cookies.Add(new HttpCookie("userName") { Value = loginModel.UserName });
            }
            if (checkResult == false)
            {
                ViewBag.ErrorMsg = "请输入用户名和密码";
                return View();
            }
            else 
            {
                return RedirectToAction("Index", "Home");
              //  return RedirectToAction("GetAllPerson", "Person");
            }
        }
	}
}