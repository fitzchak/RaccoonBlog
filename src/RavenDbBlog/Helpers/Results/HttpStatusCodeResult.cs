using System;
using System.Web.Mvc;

namespace RavenDbBlog.Controllers
{
    public class HttpStatusCodeResult : ViewResult
    {
        public int StatusCode { get; private set; }
        public string StatusDescription { get; private set; }

        public HttpStatusCodeResult(int statusCode) : this(statusCode, null) { }

        public HttpStatusCodeResult(int statusCode, string statusDescription)
        {
            this.StatusCode = statusCode;
            this.StatusDescription = statusDescription;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.HttpContext.Response.StatusCode = this.StatusCode;
            if (this.StatusDescription != null)
            {
                context.HttpContext.Response.StatusDescription = this.StatusDescription;
            }
            // 1. Uncomment this to use the existing Error.ascx / Error.cshtml to view as an error or
            // 2. Uncomment this and change to any custom view and set the name here or simply
            // 3. (Recommended) Let it commented and the ViewName will be the current controller view action and on your view (or layout view even better) show the @ViewBag.Message to produce an inline message that tell the Not Found or Unauthorized
            //this.ViewName = "Error";
            this.ViewBag.Message = context.HttpContext.Response.StatusDescription;
            base.ExecuteResult(context);
        }
    }
}