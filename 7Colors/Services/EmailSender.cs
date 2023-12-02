
using System.Net.Mail;
using System.Net;
using Azure.Core;
using MimeKit;

namespace _7Colors.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public Task SendEmailAsync(string toAddress, string subject, string body)
        {
            string smtpHost = _configuration["SmtpSettings:Host"]!;
            int smtpPort = _configuration.GetValue<int>("SmtpSettings:Port");
            string smtpUsername = _configuration["SmtpSettings:Username"]!;
            string smtpPassword = _configuration["SmtpSettings:Password"]!;
            bool enableSsl = _configuration.GetValue<bool>("SmtpSettings:EnableSsl");

            using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = enableSsl;

                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(smtpUsername);
                mailMessage.To.Add(toAddress);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                try
                {
                    smtpClient.Send(mailMessage);
                    Console.WriteLine("Email sent successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send email. Error: " + ex.Message);
                }
                return Task.CompletedTask;
            }
        }
    }

//public Task SendEmailAsync(string email, string subject, string htmlMessage)
//{
//    var emailToSend = new MimeMessage();
//    emailToSend.From.Add(MailboxAddress.Parse("hello@dotnetmastery.com"));
//    emailToSend.To.Add(MailboxAddress.Parse(email));
//    emailToSend.Subject = subject;
//    emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

//    //send email
//    using (var emailClient = new SmtpClient())
//    {
//        emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
//        emailClient.Authenticate("electronicsmf@aol.com", "peru1982");
//        emailClient.Send(emailToSend);
//        emailClient.Disconnect(true);
//    }

//    return Task.CompletedTask;

    /*  var client = new SendGridClient(SendGridSecret);
      var from = new EmailAddress("hello@dotnetmastery.com", "Bulky Book");
      var to = new EmailAddress(email);
      var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
      return client.SendEmailAsync(msg);*/
}