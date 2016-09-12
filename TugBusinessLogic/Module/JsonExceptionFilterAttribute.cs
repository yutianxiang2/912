using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BusinessLogic.Module
{
    public class JsonExceptionFilterAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.ExceptionHandled = true;
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        errorMessage = filterContext.Exception.Message
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

        }
    }
}