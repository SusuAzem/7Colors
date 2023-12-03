using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

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

        public async Task<bool> SendMailAsync(MailData mailData)
        {
            try
            {
                using (MimeMessage emailMessage = new MimeMessage())
                {
                    MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
                    MailboxAddress emailTo = new MailboxAddress(mailData.ToName, mailData.ToId);

                    string filePath = webHost.WebRootPath + mailData.Body;
                    string emailTemplateText = File.ReadAllText(filePath);
                    emailTemplateText = string.Format(emailTemplateText,
                        mailData.ToName, 
                        DateTime.Today.Date.ToString("dd / MMMM / yyyy", new CultureInfo("ar-SA")).ConvertNumerals());

                    BodyBuilder emailBodyBuilder = new BodyBuilder()
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
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                attachmentFile.CopyTo(memoryStream);
                                var attachmentFileByteArray = memoryStream.ToArray();
                                emailBodyBuilder.Attachments.Add(attachmentFile.FileName, attachmentFileByteArray, ContentType.Parse(attachmentFile.ContentType));
                            }
                        }
                    }
                    emailMessage.To.Add(emailTo);
                    emailMessage.From.Add(emailFrom);
                    emailMessage.Subject = mailData.Subject;
                    emailMessage.Body = emailBodyBuilder.ToMessageBody();

                    using (SmtpClient mailClient = new SmtpClient())
                    {
                        await mailClient.ConnectAsync(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                        await mailClient.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
                        await mailClient.SendAsync(emailMessage);
                        await mailClient.DisconnectAsync(true);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
