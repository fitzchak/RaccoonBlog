using System;
using System.Net.Mail;

namespace RavenDbBlog.Commands
{
    public class SendEmailCommand : ICommand
    {
        private readonly MailMessage _message;

        public SendEmailCommand(MailMessage message)
        {
            _message = message;
        }

        public void Execute()
        {
            if (_message == null)
            {
                throw new InvalidOperationException("Email cannot be null");
            }

            var client = new SmtpClient();
            client.Send(_message);
        }
    }
}