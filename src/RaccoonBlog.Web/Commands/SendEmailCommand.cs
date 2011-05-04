using System.Configuration;
using System.Linq;
using System.Net.Mail;
using RaccoonBlog.Web.Common;
using RaccoonBlog.Web.Infrastructure.Commands;

namespace RaccoonBlog.Web.Commands
{
    public class SendEmailCommand : ICommand
    {
        public string Contents { get; set; }
        public string Subject { get; set; }
        

        public void Execute()
        {
            var message = new MailMessage();

            var commentsMederatorEmails = ConfigurationManager.AppSettings["OwnerEmail"];
            commentsMederatorEmails
                .Split(';')
                .Select(x => new MailAddress(x.Trim()))
                .ForEach(email => message.To.Add(email));

            var blogName = ConfigurationManager.AppSettings["BlogName"];
            message.Subject = string.Format("{0} from {1}", Subject, blogName);

            message.Body = Contents;

            message.IsBodyHtml = true;
         
            var client = new SmtpClient();
            client.Send(message);
        }
    }
}