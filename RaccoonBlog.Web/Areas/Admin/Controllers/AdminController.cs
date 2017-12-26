using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using RaccoonBlog.Web.Controllers;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
	[Authorize]
	public abstract class AdminController : RaccoonController
	{
		private IDisposable _disableAggressiveCaching;

	    public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			_disableAggressiveCaching = DocumentStore.DisableAggressiveCaching();
			base.OnActionExecuting(filterContext);
		}

	    public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			using(_disableAggressiveCaching)
				base.OnActionExecuted(filterContext);
		}
	}
}