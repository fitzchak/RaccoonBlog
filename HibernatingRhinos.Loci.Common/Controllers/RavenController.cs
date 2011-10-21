using System.Web.Mvc;
using System.Xml.Linq;
using HibernatingRhinos.Loci.Common.Extensions;
using HibernatingRhinos.Loci.Common.Tasks;
using Raven.Client;

namespace HibernatingRhinos.Loci.Common.Controllers
{
    public abstract class RavenController : Controller
    {
    	public static IDocumentStore DocumentStore
    	{
    		get { return _documentStore; }
    		set
    		{
    			if (_documentStore == null)
    			{
    				_documentStore = value;
    			}
    		}
    	}
    	private static IDocumentStore _documentStore;

        public IDocumentSession RavenSession { get; protected set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
			RavenSession = _documentStore.OpenSession();
        }

		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			if (filterContext.IsChildAction)
				return;

			using (RavenSession)
			{
				if (filterContext.Exception != null)
					return;

				if (RavenSession != null)
					RavenSession.SaveChanges();
			}

			TaskExecutor.StartExecuting();
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