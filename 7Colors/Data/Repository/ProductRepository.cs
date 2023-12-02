using _7Colors.Data.IRepository;
using _7Colors.Models;

namespace _7Colors.Data.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly AppDbContext _db;

        public ProductRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
