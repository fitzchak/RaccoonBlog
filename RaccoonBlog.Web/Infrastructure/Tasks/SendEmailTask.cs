using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Caching;
using System.Web.Instrumentation;
using System.Web.Mvc;
using System.Web.Routing;
using HibernatingRhinos.Loci.Common.Extensions;
using HibernatingRhinos.Loci.Common.Tasks;
using RaccoonBlog.Web.Models;

namespace RaccoonBlog.Web.Infrastructure.Tasks
{
	public class SendEmailTask : BackgroundTask
	{
		private readonly string replyTo;
		private readonly string subject;
		private readonly string view;
		private readonly object model;
		private readonly string sendTo;

		public SendEmailTask(
			string replyTo,
			string subject,
			string view,
			string sendTo,
			object model)
		{
			this.replyTo = replyTo;
			this.subject = subject;
			this.view = view;
			this.model = model;
			this.sendTo = sendTo;
		}

		static SendEmailTask()
		{
			// Fix: The remote certificate is invalid according to the validation procedure.
			ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
		}

		public override void Execute()
		{
			var routeData = new RouteData();
			routeData.Values.Add("controller", "MailTemplates");
			var controllerContext = new ControllerContext(new MailHttpContext(), routeData, new MailController());
			var viewEngineResult = ViewEngines.Engines.FindView(controllerContext, view, "_Layout");
			var stringWriter = new StringWriter();
			viewEngineResult.View.Render(
				new ViewContext(controllerContext, viewEngineResult.View, new ViewDataDictionary(model), new TempDataDictionary(),
								stringWriter), stringWriter);

			var mailMessage = new MailMessage
			{
				IsBodyHtml = true,
				Body = stringWriter.GetStringBuilder().ToString(),
				Subject = subject,
			};
			if (string.IsNullOrEmpty(replyTo) == false)
			{
				try
				{
					mailMessage.ReplyToList.Add(new MailAddress(replyTo));
				}
				catch
				{
					// we explicitly ignore bad reply to emails
				}
			}

			// Send a notification of the comment to the post author
			mailMessage.To.Add(sendTo);

			// Also CC the owners, if specified
            OwnerEmails.ForEach(email => mailMessage.CC.Add(email));

			using (var smtpClient = new SmtpClient())
			{
				smtpClient.Send(mailMessage);
			}

		}

        public IEnumerable<MailAddress> OwnerEmails
	    {
	        get
	        {
                var commentsMederatorEmails = DocumentSession.Load<BlogConfig>("Blog/Config").OwnerEmail;
	            return commentsMederatorEmails
	                .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
	                .Select(x => new MailAddress(x.Trim()));
	        }
	    }

		public class MailController : Controller
		{
		}

		public class MailHttpContext : HttpContextBase
		{
			private readonly Cache cache = new Cache();

			private readonly IDictionary items = new Hashtable();

			public override IDictionary Items
			{
				get { return items; }
			}

			public override Cache Cache
			{
				get { return cache; }
			}

			public override PageInstrumentationService PageInstrumentation
			{
				get { return new PageInstrumentationService(); }
			}

			public override HttpResponseBase Response
			{
				get { return new MailHttpResponse(); }
			}

			public override HttpRequestBase Request
			{
				get { return new MailHttpRequset(); }
			}
		}

		public class MailHttpResponse : HttpResponseBase
		{
			public override string ApplyAppPathModifier(string virtualPath)
			{
				return virtualPath;
			}

			public override HttpCookieCollection Cookies
			{
				get { return new HttpCookieCollection(); }
			}
		}

		public class MailHttpRequset : HttpRequestBase
		{
			public override string UserAgent
			{
				get { return "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.72 Safari/537.36"; }
			}

			public override HttpBrowserCapabilitiesBase Browser
			{
				get { return new MailBrowserCapabilities(); }
			}

			public override HttpCookieCollection Cookies
			{
				get { return new HttpCookieCollection(); }
			}

			public override bool IsLocal
			{
				get { return true; }
			}

			public override string ApplicationPath
			{
				get { return "/"; }
			}

			public override NameValueCollection ServerVariables
			{
				get { return new NameValueCollection(); }
			}

			public override Uri Url
			{
				get { return new Uri(ConfigurationManager.AppSettings["MainUrl"]); }
			}
		}

		public class MailBrowserCapabilities : HttpBrowserCapabilitiesBase
		{
			public override bool IsMobileDevice
			{
				get { return false; }
			}
		}
	}
}