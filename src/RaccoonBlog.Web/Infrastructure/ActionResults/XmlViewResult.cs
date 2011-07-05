using System.Web.Mvc;

namespace RaccoonBlog.Web.Infrastructure.ActionResults
{
	public class XmlViewResult : ViewResult
	{
		public string ETag { get; set; }

		public override void ExecuteResult(ControllerContext context)
		{
			if (ETag != null)
				context.HttpContext.Response.AddHeader("ETag", ETag);

			context.HttpContext.Response.ContentType = "text/xml";

			base.ExecuteResult(context);
		}
	}
}