using System;
using System.ChesFrame.Utility.WCF;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;

namespace System.ChesFrame.Service.Host.Controllers
{
    public class HostController : Controller
    {
        //
        // GET: /Host/
        public ActionResult Service()
        {
            var serviceHostContainer = ServiceHostContainer.Instance;
            var serviceHosts = serviceHostContainer.ServiceHosts;

            List<string> hostInfo = new List<string>();
            foreach (ServiceHost sub in serviceHosts) { 
                foreach(var m in sub.Description.Endpoints){
                    if (m.Contract.ContractType.Name != "IMetadataExchange")
                    {
                        hostInfo.Add(string.Format("{0},{1},{2}",
                           sub.Description.ServiceType.FullName,
                           m.Contract.ContractType.FullName,
                           m.Address
                           ));
                    }
                }
                //hostInfo.Add(string.Format("{0,1,2}",
                //    sub.Description.Name,
                    
                //    ));
            }

            ViewBag.HostInfo = hostInfo;

            return View();
        }
	}
}