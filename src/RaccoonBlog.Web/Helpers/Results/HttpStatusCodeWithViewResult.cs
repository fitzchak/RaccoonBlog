using System;
using System.Web.Mvc;

namespace RaccoonBlog.Web.Helpers.Results
{
    public class HttpStatusCodeWithViewResult : ViewResult
    {
        public int StatusCode { get; private set; }
        public string StatusDescription { get; private set; }

        public HttpStatusCodeWithViewResult(int statusCode) : this(statusCode, null) { }

        public HttpStatusCodeWithViewResult(int statusCode, string statusDescription)
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
            ViewName = StatusCode.ToString();
            base.ExecuteResult(context);
        }
    }
}