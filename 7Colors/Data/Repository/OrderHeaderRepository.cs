using _7Colors.Data.IRepository;
using _7Colors.Models;

namespace _7Colors.Data.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly AppDbContext _db;

        public OrderHeaderRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
     
        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (paymentStatus != null)
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }

        }
        public void UpdateStripePaymentID(int orderheaderId, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == orderheaderId);
            orderFromDb!.PaymentDate = DateTime.Now;
            orderFromDb.SessionId = sessionId;
            orderFromDb.PaymentIntentId = paymentIntentId;
        }
    }
}
