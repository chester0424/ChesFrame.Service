using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace System.ChesFrame.Utility.SiteMap
{
    public static class HtmlExtend4MvcSiteMap
    {
        public static MvcHtmlString RenderNavigation(this HtmlHelper helper, string siteMapProviderName = "")
        {
            var viewContent = helper.ViewContext;
            var requestContext = helper.ViewContext.RequestContext;
            var a = requestContext.RouteData;
            return new MvcHtmlString("");
        }
    }
}
