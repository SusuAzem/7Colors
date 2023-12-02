namespace _7Colors.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toAddress, string subject, string body);
    }
}