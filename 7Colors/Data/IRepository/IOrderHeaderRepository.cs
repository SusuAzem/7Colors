using _7Colors.Models;

namespace _7Colors.Data.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {       
        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        void UpdateStripePaymentID(int orderheaderId, string sessionId, string paymentIntentId);
    }
}
