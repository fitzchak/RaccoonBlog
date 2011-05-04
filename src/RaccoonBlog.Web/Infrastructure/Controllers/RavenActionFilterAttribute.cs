using System.Web.Mvc;
using RaccoonBlog.Web.Infrastructure.Raven;

namespace RaccoonBlog.Web.Infrastructure.Controllers
{
    /// <summary>
    /// This filter will manage the session for all of the controllers that needs a Raven Document Session.
    /// It does so by automatically injecting a session to the first public property of type IDocumentSession available
    /// on the controller.
    /// </summary>
    public class RavenActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DocumentStoreHolder.TryAddSession(filterContext.Controller); 
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            DocumentStoreHolder.TryComplete(filterContext.Controller, filterContext.Exception == null);
        }
    }
}