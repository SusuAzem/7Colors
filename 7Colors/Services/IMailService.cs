namespace _7Colors.Services
{
    public interface IMailService
    {
        Task<bool> SendMailAsync(MailData mailData);
    }
}
