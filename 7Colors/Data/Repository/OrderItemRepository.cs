using _7Colors.Data.IRepository;
using _7Colors.Models;

namespace _7Colors.Data.Repository
{
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        private readonly AppDbContext _db;

        public OrderItemRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }       
    }
}
