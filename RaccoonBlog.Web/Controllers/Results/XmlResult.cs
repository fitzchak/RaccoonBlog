using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

namespace RaccoonBlog.Web.Controllers.Results
{
    public class XmlResult : ActionResult
	{
		private readonly XDocument _document;
		private readonly string _etag;

		public XmlResult(XDocument document, string etag)
		{
			_document = document;
			_etag = etag;
		}

		public override void ExecuteResult(ActionContext context)
		{
			if (_etag != null)
                context.HttpContext.Response.Headers.Add("ETag", _etag);

			context.HttpContext.Response.ContentType = "text/xml";

			using (var xmlWriter = XmlWriter.Create(context.HttpContext.Response.Body))
			{
				_document.WriteTo(xmlWriter);
				xmlWriter.Flush();
			}
		}
	}
}