using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using DataAnnotationsExtensions.ClientValidation;
using FluentScheduler;
using Glimpse.RavenDb;

using HibernatingRhinos.Loci.Common.Controllers;
using HibernatingRhinos.Loci.Common.Tasks;
using NLog.Fluent;
using RaccoonBlog.Web.Areas.Admin.Controllers;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Helpers.Binders;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Indexes;
using RaccoonBlog.Web.Infrastructure.Jobs;
using Raven.Abstractions.Logging;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace RaccoonBlog.Web
{
	public class MvcApplication : HttpApplication
	{
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

		public MvcApplication()
		{
			BeginRequest += (sender, args) =>
			{
				BundleConfig.RegisterThemeBundles(HttpContext.Current, BundleTable.Bundles);
				HttpContext.Current.Items["CurrentRequestRavenSession"] = RavenController.DocumentStore.OpenSession();
			};
			EndRequest += (sender, args) =>
			{
				using (var session = (IDocumentSession)HttpContext.Current.Items["CurrentRequestRavenSession"])
				{
					if (session == null)
						return;

					if (Server.GetLastError() != null)
						return;

					session.SaveChanges();
				}
				TaskExecutor.StartExecuting();
			};
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			new RouteConfigurator(RouteTable.Routes).Configure();

			InitializeDocumentStore();
			LogManager.GetCurrentClassLogger().Info("Started Raccoon Blog");

			ModelBinders.Binders.Add(typeof(CommentCommandOptions), new RemoveSpacesEnumBinder());
			ModelBinders.Binders.Add(typeof(Guid), new GuidBinder());

			DataAnnotationsModelValidatorProviderExtensions.RegisterValidationExtensions();

			AutoMapperConfiguration.Configure();
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			RavenController.DocumentStore = DocumentStore;
			TaskExecutor.DocumentStore = DocumentStore;
			Profiler.AttachTo((DocumentStore)DocumentStore);

			// In case the versioning bundle is installed, make sure it will version
			// only what we opt-in to version
			using (var s = DocumentStore.OpenSession())
			{
				s.Store(new
				{
					Exclude = true,
					Id = "Raven/Versioning/DefaultConfiguration",
				});
				s.SaveChanges();
			}

            JobManager.JobException += JobExceptionHandler;
            JobManager.Initialize(new SocialNetworkIntegrationJobsRegistry());
        }

		public static IDocumentStore DocumentStore { get; private set; }

        static void JobExceptionHandler(JobExceptionInfo info, FluentScheduler.UnhandledExceptionEventArgs e)
        {
            _log.FatalException($"Error executing background job {info.Name}.", e.ExceptionObject);
        }

        private static void InitializeDocumentStore()
		{
			if (DocumentStore != null) return; // prevent misuse

		    DocumentStore = new DocumentStore
			{
				ConnectionStringName = "RavenDB",
			}.Initialize();

            TryCreatingIndexesOrRedirectToErrorPage();
		}

		private static void TryCreatingIndexesOrRedirectToErrorPage()
		{
			try
			{
				IndexCreation.CreateIndexes(typeof(Tags_Count).Assembly, DocumentStore);
			}
			catch (WebException e)
			{
				var socketException = e.InnerException as SocketException;
				if (socketException == null)
					throw;

				switch (socketException.SocketErrorCode)
				{
					case SocketError.AddressNotAvailable:
					case SocketError.NetworkDown:
					case SocketError.NetworkUnreachable:
					case SocketError.ConnectionAborted:
					case SocketError.ConnectionReset:
					case SocketError.TimedOut:
					case SocketError.ConnectionRefused:
					case SocketError.HostDown:
					case SocketError.HostUnreachable:
					case SocketError.HostNotFound:
						HttpContext.Current.Response.Redirect("~/RavenNotReachable.htm");
						break;
					default:
						throw;
				}
			}
		}
	}
}