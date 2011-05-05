using System.Net.Mail;
using Mvc.Mailer;
using RaccoonBlog.Web.Infrastructure.Commands;

namespace RaccoonBlog.Web.Commands
{
    public class SendEmailCommand : ICommand
    {
        private readonly MailMessage mailMessage;

        public SendEmailCommand(MailMessage mailMessage)
        {
            this.mailMessage = mailMessage;
        }

        public void Execute()
        {
            mailMessage.Send();
        }
    }
}