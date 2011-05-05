using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Mvc.Mailer;
using System.Net.Mail;
using RaccoonBlog.Web.Common;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Mailers
{ 
    public class MailTemplates : MailerBase  
	{
		public MailTemplates()
		{
		    MasterName = "_Layout";
		}
		
		public virtual MailMessage NewComment(NewCommentEmailViewModel viewModel)
		{
            var blogName = ConfigurationManager.AppSettings["BlogName"];

            var message = new MailMessage
                              {
                                  IsBodyHtml = true,
                                  Subject = string.Format("Comment on: {0} from {1}", viewModel.PostTitle, blogName)
                              };

            var commentsMederatorEmails = ConfigurationManager.AppSettings["OwnerEmail"];
            commentsMederatorEmails
                .Split(';')
                .Select(x => new MailAddress(x.Trim()))
                .ForEach(email => message.To.Add(email));

		    ViewData = new ViewDataDictionary(viewModel);
            PopulateBody(message, "NewComment");

            return message;
		}
	}
}