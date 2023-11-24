namespace _7Colors.Utility
{
    public interface IEmailSender
    {
        void SendEmail(string toAddress, string subject, string body);
    }
}