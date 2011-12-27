using System.Web.Mvc;
using System.Xml.Linq;
using HibernatingRhinos.Loci.Common.Extensions;
using Raven.Client;

namespace HibernatingRhinos.Loci.Common.Controllers
{
	public abstract class RavenController : Controller
	{
		public static IDocumentStore DocumentStore { get; set; }

		public IDocumentSession RavenSession { get; protected set; }

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			RavenSession = (IDocumentSession)HttpContext.Items["CurrentRequestRavenSession"];
		}

		protected HttpStatusCodeResult HttpNotModified()
		{
			return new HttpStatusCodeResult(304);
		}

		protected ActionResult Xml(XDocument xml, string etag)
		{
			return new XmlResult(xml, etag);
		}
	}
}