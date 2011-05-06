using System;
using System.Web;
using System.Web.Mvc;
using Elmah;

namespace RaccoonBlog.Web.Helpers
{
    public class ElmahHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
        	View = "500";
			ErrorLog.GetDefault(HttpContext.Current).Log(new Error(context.Exception, HttpContext.Current));
			base.OnException(context);
        }
    }
}