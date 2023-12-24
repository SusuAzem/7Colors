using System.Net.Mail;

namespace _7Colors.Services
{
    public interface IMailService
    {
        Task<bool> SendMailAsync(MailData mailData);
        Task<bool> ReceiveEmailAsync(MailData mailData);
    }
}
