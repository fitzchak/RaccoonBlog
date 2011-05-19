using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Elmah;
using RaccoonBlog.Web.Infrastructure;
using RaccoonBlog.Web.Infrastructure.Commands;

namespace RaccoonBlog.Web.Commands
{
    public class SendEmailCommand : ICommand
    {
    	private readonly string replyTo;
    	private readonly string subject;
    	private readonly string view;
    	private readonly object model;

        public SendEmailCommand(
			string replyTo,
			string subject,
			string view, 
			object model)
        {
        	this.replyTo = replyTo;
        	this.subject = subject;
        	this.view = view;
        	this.model = model;
        }

    	public void Execute()
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
			if(string.IsNullOrEmpty(replyTo) == false)
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

			var commentsMederatorEmails = ConfigurationManager.AppSettings["OwnerEmail"];
			commentsMederatorEmails
				.Split(';')
				.Select(x => new MailAddress(x.Trim()))
				.ForEach(email => mailMessage.To.Add(email));


    		using(var smtpClient = new SmtpClient())
    		{
    			smtpClient.Send(mailMessage);
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
				get
				{
					return new MailHttpResponse();
				}
			}

			public override HttpRequestBase Request
			{
				get
				{
					return new HttpRequestWrapper(new HttpRequest("", ConfigurationManager.AppSettings["MainUrl"], ""));
				}
			}
		}

		public class MailHttpResponse : HttpResponseBase
		{
			public override string ApplyAppPathModifier(string virtualPath)
			{
				return virtualPath;
			}
		}
    }

}
