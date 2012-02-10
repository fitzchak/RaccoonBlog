using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RaccoonBlog.Web.Controllers;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Listeners;
using Rhino.Mocks;

namespace RaccoonBlog.IntegrationTests.Web.Controllers
{
	public abstract class RaccoonControllerTests : IDisposable
	{
		private readonly EmbeddableDocumentStore documentStore;
		protected ControllerContext ControllerContext { get; set; }

		protected RaccoonControllerTests()
		{
			documentStore = new EmbeddableDocumentStore
			                {
			                	RunInMemory = true
			                };

			documentStore.RegisterListener(new NoStaleQueriesAllowed());
			documentStore.Initialize();
		}

		protected void SetupData(Action<IDocumentSession> action)
		{
			using (var session = documentStore.OpenSession())
			{
				action(session);
				session.SaveChanges();
			}
		}

		public class NoStaleQueriesAllowed : IDocumentQueryListener
		{
			public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
			{
				queryCustomization.WaitForNonStaleResults();
			}
		}

		public void Dispose()
		{
			documentStore.Dispose();
		}

		protected void ExecuteAction<TController>(Action<TController> action) 
			where TController: RaccoonController, new()
		{
			var controller = new TController {RavenSession = documentStore.OpenSession()};

			var httpContext = MockRepository.GenerateStub<HttpContextBase>();
			httpContext.Stub(x => x.Response).Return(MockRepository.GenerateStub<HttpResponseBase>());
			ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);
			controller.ControllerContext = ControllerContext;

			action(controller);

			controller.RavenSession.SaveChanges();
		}
	}
}