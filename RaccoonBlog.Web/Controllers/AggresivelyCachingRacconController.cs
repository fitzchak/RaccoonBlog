using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaccoonBlog.Web.Controllers
{
	public abstract class AggresivelyCachingRacconController : RaccoonController
	{
		IDisposable _aggressivelyCacheFor;

	    public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);

			_aggressivelyCacheFor = RavenSession.Advanced.DocumentStore.AggressivelyCacheFor(CacheDuration);
		}

		protected abstract TimeSpan CacheDuration { get; }

		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			base.OnActionExecuted(filterContext);

			if (_aggressivelyCacheFor != null)
			{
				_aggressivelyCacheFor.Dispose();
				_aggressivelyCacheFor = null;
			}
		}
	}
}