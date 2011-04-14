using System;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using RavenDbBlog.Common;

namespace RavenDbBlog.Commands
{
    public class SendEmailCommand : ICommand
    {
        public string Contents { get; set; }
        public string Subject { get; set; }
        

        public void Execute()
        {
            var message = new MailMessage();

            var commentsMederatorEmails = ConfigurationManager.AppSettings["CommentsMederatorEmails"];
            commentsMederatorEmails
                .Split(';')
                .Select(x => new MailAddress(x.Trim()))
                .ForEach(email => message.To.Add(email));

            var blogName = ConfigurationManager.AppSettings["blogName"];
            message.Subject = string.Format("{0} from {1}", Subject, blogName);

            message.Body = Contents;

            message.IsBodyHtml = true;
         
            var client = new SmtpClient();
            client.Send(message);
        }
    }
}