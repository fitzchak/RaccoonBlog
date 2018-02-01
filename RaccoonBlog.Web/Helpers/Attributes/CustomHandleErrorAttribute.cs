using System.Web.Mvc;
using System.Web.Routing;

using NLog;

namespace RaccoonBlog.Web.Helpers.Attributes
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public override void OnException(ExceptionContext filterContext)
        {
            Log.Error(filterContext.Exception, "Unexpected error occured.");

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                base.OnException(filterContext);
                return;
            }

            if (filterContext.HttpContext.IsCustomErrorEnabled == false)
            {
                return;
            }

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
            {
                action = MVC.Error.ActionNames.Error,
                controller = MVC.Error.Name
            }));
            filterContext.ExceptionHandled = true;


            base.OnException(filterContext);
        }
    }
}