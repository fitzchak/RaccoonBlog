using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using HibernatingRhinos.Loci.Common.Extensions;

namespace HibernatingRhinos.Loci.Common.Tasks
{
	public class SendEmailTask : BackgroundTask
	{
		public string ReplyTo { get; set; }
		public string Subject { get; set; }
		public string HtmlBody { get; set; }
		public string[] SendTo { get; set; }
		public string[] Cc { get; set; }
		public string[] Bcc { get; set; }

		public static string RenderBodyUsingView(string viewName, object model)
		{
			return RenderBodyUsingView(viewName, model, new MailHttpContext());
		}

		public static string RenderBodyUsingView(string viewName, object model, HttpContextBase httpContext)
		{
			var routeData = new RouteData();
			routeData.Values.Add("controller", "MailTemplates");
			var controllerContext = new ControllerContext(httpContext, routeData, new MailController());
			var viewEngineResult = ViewEngines.Engines.FindView(controllerContext, viewName, "_Layout");
			var stringWriter = new StringWriter();
			viewEngineResult.View.Render(
				new ViewContext(controllerContext, viewEngineResult.View, new ViewDataDictionary(model), new TempDataDictionary(),
								stringWriter), stringWriter);

			return stringWriter.GetStringBuilder().ToString();
		}

		public override void Execute()
		{
			var mailMessage = new MailMessage
			                  	{
			                  		IsBodyHtml = true,
									Body = HtmlBody,
			                  		Subject = Subject,
			                  	};
			if (string.IsNullOrEmpty(ReplyTo) == false)
			{
				try
				{
					mailMessage.ReplyToList.Add(new MailAddress(ReplyTo));
				}
				catch
				{
					// we explicitly ignore bad reply to emails
				}
			}

			// Send a notification of the comment to the post author
			if (SendTo != null) SendTo.ForEach(x => mailMessage.To.Add(x));
			if (Cc != null) Cc.ForEach(email => mailMessage.CC.Add(email));
			if (Bcc != null) Bcc.ForEach(email => mailMessage.Bcc.Add(email));

			using (var smtpClient = new SmtpClient())
			{
#if !DEBUG
				smtpClient.Send(mailMessage);
#endif
			}

		}

		public class MailController : Controller
		{
		}

		public class MailHttpContext : HttpContextBase
		{
			private readonly IDictionary items = new Hashtable();

			public override IDictionary Items
			{
				get { return items; }
			}

			public override System.Web.Caching.Cache Cache
			{
				get { return HttpRuntime.Cache; }
			}

			public override HttpResponseBase Response
			{
				get { return new MailHttpResponse(); }
			}

			public override HttpRequestBase Request
			{
				get { return new HttpRequestWrapper(new HttpRequest("", ConfigurationManager.AppSettings["MainUrl"], "")); }
			}
		}

		public class MailHttpResponse : HttpResponseBase
		{
			public override string ApplyAppPathModifier(string virtualPath)
			{
				return virtualPath;
			}
		}

		public static string[] SpliceEmailAddresses(string emails)
		{
			return emails
				.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
				.Select(x => x.Trim())
				.ToArray();
		}
	}
}
