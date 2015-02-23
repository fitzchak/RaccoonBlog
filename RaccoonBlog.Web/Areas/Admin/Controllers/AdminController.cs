using System;
using System.Web.Mvc;
using RaccoonBlog.Web.Controllers;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
	[Authorize]
	public abstract partial class AdminController : RaccoonController
	{
		private IDisposable disableAggressiveCaching;

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			disableAggressiveCaching = DocumentStore.DisableAggressiveCaching();
			base.OnActionExecuting(filterContext);
		}

		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			using(disableAggressiveCaching)
				base.OnActionExecuted(filterContext);
		}
	}
}