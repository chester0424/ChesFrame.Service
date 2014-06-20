using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace System.ChesFrame.Web.Common
{
    public class AjaxRequestExceptionFilter : FilterAttribute, IExceptionFilter
    {
        //对为处理的异常进行处理
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())//只对AjaxRequest 的异常进行处理
            {

                var exceptionMsg = filterContext.Exception.Message;
                AjaxExceptionInfo info = new AjaxExceptionInfo()
                {
                    ErrorMsg = exceptionMsg
                };

                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                var result = jsonSerializer.Serialize(info);

                var response = filterContext.HttpContext.Response;
                response.AddHeader("Content-Type", "application/json; charset=utf-8");

                response.Write(result);

                filterContext.ExceptionHandled = true;//表示已经处理了异常
            }
        }
    }

    public class AjaxExceptionInfo 
    {
        public string ErrorMsg { get;set;}
    }
}