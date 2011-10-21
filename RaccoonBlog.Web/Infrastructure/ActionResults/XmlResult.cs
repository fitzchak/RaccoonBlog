using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace RaccoonBlog.Web.Infrastructure.ActionResults
{
	public class XmlResult : ActionResult
	{
		private readonly XDocument document;
		private readonly string etag;

		public XmlResult(XDocument document, string etag)
		{
			this.document = document;
			this.etag = etag;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			if (etag != null)
				context.HttpContext.Response.AddHeader("ETag", etag);

			context.HttpContext.Response.ContentType = "text/xml";

			using (var xmlWriter = XmlWriter.Create(context.HttpContext.Response.OutputStream))
			{
				document.WriteTo(xmlWriter);
				xmlWriter.Flush();
			}
		}
	}
}
