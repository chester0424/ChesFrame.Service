using System;
using System.ChesFrame.Utility.WCF;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace System.ChesFrame.Service.Host
{
    public class MvcApplication : System.Web.HttpApplication
    {
        ServiceHostContainer serviceHostContainer = ServiceHostContainer.Instance;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);


            serviceHostContainer.LoadServieHost();
            serviceHostContainer.ServiceHostOpen();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            serviceHostContainer.ServiceHostClose();
        }
    }
}
