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

            context.HttpContext.Response.StatusCode = StatusCode;
            if (StatusDescription != null)
            {
                context.HttpContext.Response.StatusDescription = StatusDescription;
            }
            this.ViewName = StatusCode.ToString();
            base.ExecuteResult(context);
        }
    }
}