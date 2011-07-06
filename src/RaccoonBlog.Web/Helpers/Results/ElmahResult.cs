using System.Web;
using System.Web.Mvc;

namespace RaccoonBlog.Web.Helpers.Results
{
	public class ElmahResult : ActionResult
	{
		private readonly string _resouceType;

		public ElmahResult(string resouceType)
		{
			_resouceType = resouceType;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			var url = new UrlHelper(HttpContext.Current.Request.RequestContext);
			var factory = new Elmah.ErrorLogPageFactory();
			if (!string.IsNullOrEmpty(_resouceType))
			{
				var pathInfo = "." + _resouceType;
			    var action = url.Action("Index", "Elmah", new {type = (string) null});
				HttpContext.Current.RewritePath(action, pathInfo, HttpContext.Current.Request.QueryString.ToString());
			}

			var handler = factory.GetHandler(HttpContext.Current, null, null, null);

			handler.ProcessRequest(HttpContext.Current);
		}
	}
}