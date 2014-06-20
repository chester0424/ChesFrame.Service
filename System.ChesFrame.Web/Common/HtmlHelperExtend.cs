using System;
using System.ChesFrame.Utility.SiteMap;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace System.ChesFrame.Web.Common
{
    public static class HtmlHelperExtend
    {
        public static MvcHtmlString RenderNavigation(this HtmlHelper helper, string siteMapProviderName = "")
        {
            var routeData = (((System.Web.Mvc.ControllerContext)(helper.ViewContext))).RouteData;
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];

            var mvcSiteMapConfig = MvcSitemapConfigManager.Instance.SiteMapConfig;
            var mvcSiteMapNodes = mvcSiteMapConfig.MvcSiteMapNodes;

            Func<string, string, string, string> renderLink = (Func<string, string, string, string>)(
                    (controllerArg, actionArg, titleArg) =>
                    {
                        return string.Format("<a href='~/{0}/{1}/'>{2}</a>", controllerArg, actionArg, titleArg);
                    }
                );

            Func<string, string> renderSpan = (Func<string, string>)(
                   (titleArg) =>
                   {
                       return string.Format("<span>{0}</span>", titleArg);
                   }
               );

            foreach (var sub in mvcSiteMapNodes)
            {
               // sub.Controller
            }

            return new MvcHtmlString("");
        }

        public static MvcHtmlString RenderMainMenu(this HtmlHelper helper)
        {
            return null;
        }

        public static MvcHtmlString RenderSubMenu(this HtmlHelper helper)
        { return null; }


        #region 页面级JS调用

        public static IHtmlString PageScriptRender(this HtmlHelper helper,ViewContext viewContext)
        {
            var controller = viewContext.RouteData.Route.GetRouteData(viewContext.HttpContext).Values["controller"];
            var action = viewContext.RouteData.Route.GetRouteData(viewContext.HttpContext).Values["action"];

            string path =string.Format( "<script src=\"~/Content/Scripts/pages/{0}/{1}.js\"></script>",controller,action);
            return   new HtmlString(path);
        }

        #endregion
    }
}