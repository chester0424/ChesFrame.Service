using System;
using System.ChesFrame.Web.Common;
using System.ChesFrame.Web.Facade;
using System.ChesFrame.Web.Model;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace System.ChesFrame.Web.Controllers
{
    public class PersonController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetPersonByPage()
        {
            try
            {
                var request = Request["queryKey"];
                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                var result = jsonSerializer.Deserialize<PersonQueryModel>(request);

                var data = PersonFacade.GetPersonByPage(result);
                result.PageInfo.Count = result.PageInfo.Count;
                return Json(new QueryResult<PersonModel>() { PageInfo = result.PageInfo, Data = data });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ActionResult GetAllPerson()
        {
            var allPerson = PersonFacade.GetAllPerson();
            ViewBag.AllPerson = allPerson;
            return View();

        }

        public ActionResult Edit()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Edit(PersonModel person)
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetCount()
        {
            var count = PersonFacade.GetCount();
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        [AjaxRequestExceptionFilter]
        public ActionResult QueryPerson()
        {
            var request = Request["queryKey"];
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            var result = jsonSerializer.Deserialize<PersonQueryModel>(request);

            var data = PersonFacade.QueryPerson(result);

            result.PageInfo.Count = result.PageInfo.Count;
            return Json(new QueryResult<PersonModel>() { PageInfo = result.PageInfo, Data = data });
           
        }

        public ActionResult FormInfoToEntity(PersonQueryModel model)
        {
            //var request = Request["queryKey"];
            //JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            //var result = jsonSerializer.Deserialize<PersonQueryModel>(request);
            return Json(model.Name);
        }
        
        /*
         * Ajax 提交数据是以form的方式提交，所以对于这部分参数的访问可以使用Request.Form["key"],Request["key"],Request.Param["Key"]
         */
        public ActionResult FormInfoToEntity2()
        {
            try
            {
                var request = Request["queryKey"];
                //request = Uri.UnescapeDataString(request);
                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                var result = jsonSerializer.Deserialize<PersonQueryModel>(request);
                return Json(result.Name);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
	}
}