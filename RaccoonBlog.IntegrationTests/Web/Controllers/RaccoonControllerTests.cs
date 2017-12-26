using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RaccoonBlog.Web.Controllers;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.TestDriver;
using Rhino.Mocks;

namespace RaccoonBlog.IntegrationTests.Web.Controllers
{
	public abstract class RaccoonControllerTests : RavenTestDriver<TestsServerLocator>
	{
	    private readonly IDocumentStore _documentStore;

	    protected ControllerContext ControllerContext { get; set; }

		protected RaccoonControllerTests()
		{
		    _documentStore = GetDocumentStore();
		}

	    protected override void PreInitialize(IDocumentStore documentStore)
	    {
	        documentStore.OnBeforeQueryExecuted += (sender, args) => { args.QueryCustomization.WaitForNonStaleResults(); };
	    }

	    protected void SetupData(Action<IDocumentSession> action)
		{
			using (var session = _documentStore.OpenSession())
			{
				action(session);
				session.SaveChanges();
			}
		}

		protected void ExecuteAction<TController>(Action<TController> action) 
			where TController: RaccoonController, new()
		{
			var controller = new TController {RavenSession = _documentStore.OpenSession()};

			var httpContext = MockRepository.GenerateStub<HttpContextBase>();
			httpContext.Stub(x => x.Response).Return(MockRepository.GenerateStub<HttpResponseBase>());
			ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);
			controller.ControllerContext = ControllerContext;

			action(controller);

			controller.RavenSession.SaveChanges();
		}
	}
}