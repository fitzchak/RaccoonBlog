using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RaccoonBlog.Web.Controllers;
using RaccoonBlog.Web.Helpers.Attributes;
using RaccoonBlog.Web.Helpers.Binders;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Indexes;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Client.MvcIntegration;

namespace RaccoonBlog.Web
{
	public class MvcApplication : HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			//filters.Add(new ElmahHandleErrorAttribute());
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			InitializeDocumentStore();
			new RouteConfigurator(RouteTable.Routes).Configure();
			ModelBinders.Binders.Add(typeof (CommentCommandOptions), new RemoveSpacesEnumBinder());
			ModelBinders.Binders.Add(typeof (Guid), new GuidBinder());

			AutoMapperConfiguration.Configure();
		}

		public static IDocumentStore DocumentStore { get; private set; }

		private static void InitializeDocumentStore()
		{
			DocumentStore = new DocumentStore
			                	{
			                		ConnectionStringName = "RavenDB"
			                	}.Initialize();

			IndexCreation.CreateIndexes(typeof (Tags_Count).Assembly, DocumentStore);

			//RavenProfilingHandler.SourcePath = @"C:\Work\ravendb\Raven.Client.MvcIntegration";

			RavenProfiler.InitializeFor(MvcApplication.DocumentStore,
			                            //Fields to filter out of the output
			                            "Email", "HashedPassword", "AkismetKey", "GoogleAnalyticsKey", "ShowPostEvenIfPrivate",
			                            "PasswordSalt", "UserHostAddress");
		}
	}
}
