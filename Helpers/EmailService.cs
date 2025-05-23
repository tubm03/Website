using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;

namespace PetStoreProject.Helpers
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            SmtpClient smtpCilent = new SmtpClient(_configuration["MailSettings:Host"]);
            smtpCilent.Port = int.Parse(_configuration["MailSettings:Port"]);
            smtpCilent.Credentials = new NetworkCredential(_configuration["MailSettings:UserName"], _configuration["MailSettings:Password"]);
            smtpCilent.EnableSsl = true;

            MailMessage message = new MailMessage()
            {
                From = new MailAddress(_configuration["MailSettings:From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                SubjectEncoding = System.Text.Encoding.UTF8,
                BodyEncoding = System.Text.Encoding.UTF8,
            };
            message.To.Add(toEmail);

            smtpCilent.Send(message);
        }
    }
}
