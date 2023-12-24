using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

using Org.BouncyCastle.Asn1.Pkcs;

using System.Globalization;


namespace _7Colors.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly IWebHostEnvironment webHost;

        public MailService(IOptions<MailSettings> mailSettingsOptions, IWebHostEnvironment webHost)
        {
            _mailSettings = mailSettingsOptions.Value;
            this.webHost = webHost;
        }

        public async Task<bool> ReceiveEmailAsync(MailData mailData)
        {
            using var emailClient = new ImapClient();
            emailClient.Connect(_mailSettings.Host, _mailSettings.IMAPPort, _mailSettings.EnableSsl);

            emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

            emailClient.Authenticate(_mailSettings.UserName, _mailSettings.Password);

            using MimeMessage emailMessage = new();
            MailboxAddress emailTo = new(_mailSettings.SenderName, _mailSettings.SenderEmail);
            MailboxAddress emailFrom = new(mailData.ToName, mailData.ToId);

            string emailText = mailData.Body + Environment.NewLine +
                DateTime.Today.Date.ToString("dd / MMMM / yyyy", new CultureInfo("ar-SA")).ConvertNumerals();

            BodyBuilder emailBodyBuilder = new()
            {
                TextBody = emailText,
            };
            emailMessage.To.Add(emailTo);
            emailMessage.From.Add(emailFrom);
            emailMessage.Subject = mailData.Subject;
            emailMessage.Body = emailBodyBuilder.ToMessageBody();
            try
            {
                await emailClient.Inbox.AppendAsync(emailMessage);
                emailClient.Disconnect(true);
                return true;
            }
            catch (Exception)
            {
                await emailClient.GetFolder("DRAFT").AppendAsync(emailMessage);
                emailClient.Disconnect(true);
                return false;
            }
        }

        public async Task<bool> SendMailAsync(MailData mailData)
        {
            try
            {
                using MimeMessage emailMessage = new();
                MailboxAddress emailFrom = new(_mailSettings.SenderName, _mailSettings.SenderEmail);
                MailboxAddress emailTo = new(mailData.ToName, mailData.ToId);

                string filePath = webHost.WebRootPath + mailData.Body;
                string emailTemplateText = File.ReadAllText(filePath);
                emailTemplateText = string.Format(emailTemplateText,
                    mailData.ToName,
                    DateTime.Today.Date.ToString("dd / MMMM / yyyy", new CultureInfo("ar-SA")).ConvertNumerals());

                BodyBuilder emailBodyBuilder = new()
                {
                    HtmlBody = emailTemplateText,
                    TextBody = "مرحبا بك معنا في فريق الألوان السبعة",
                };
                if (mailData.EmailAttachments != null)
                {
                    foreach (var attachmentFile in mailData.EmailAttachments)
                    {
                        if (attachmentFile.Length == 0)
                        {
                            continue;
                        }
                        using MemoryStream memoryStream = new();
                        attachmentFile.CopyTo(memoryStream);
                        var attachmentFileByteArray = memoryStream.ToArray();
                        emailBodyBuilder.Attachments.Add(attachmentFile.FileName, attachmentFileByteArray, ContentType.Parse(attachmentFile.ContentType));
                    }
                }
                emailMessage.To.Add(emailTo);
                emailMessage.From.Add(emailFrom);
                emailMessage.Subject = mailData.Subject;
                emailMessage.Body = emailBodyBuilder.ToMessageBody();

               SmtpClient mailClient = new();
                await mailClient.ConnectAsync(_mailSettings.Host, _mailSettings.SMTPPort, _mailSettings.EnableSsl);
                //await mailClient.ConnectAsync(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await mailClient.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
                await mailClient.SendAsync(emailMessage);
                await mailClient.DisconnectAsync(true);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
