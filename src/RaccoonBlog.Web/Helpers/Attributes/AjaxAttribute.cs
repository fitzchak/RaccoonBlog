using System;
using System.Web.Mvc;
using RavenDbBlog.Controllers;

namespace RavenDbBlog.Helpers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AjaxOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            if (!request.IsAjaxRequest())
                filterContext.Result = new HttpNotFoundWithViewResult("Only Ajax calls are permitted.");
        } 
    }
}