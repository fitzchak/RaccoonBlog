using System.Net.Mail;
using Mvc.Mailer;
using RaccoonBlog.Web.Infrastructure.Commands;

namespace RaccoonBlog.Web.Commands
{
    public class SendEmailCommand : ICommand
    {
        private readonly MailMessage _mailMessage;

        public SendEmailCommand(MailMessage mailMessage)
        {
            _mailMessage = mailMessage;
        }

        public void Execute()
        {
            _mailMessage.Send();
        }
    }
}