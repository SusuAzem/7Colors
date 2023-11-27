namespace _7Colors.Services
{
    public interface IEmailSender
    {
        void SendEmail(string toAddress, string subject, string body);
    }
}