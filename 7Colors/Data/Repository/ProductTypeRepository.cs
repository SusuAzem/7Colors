using _7Colors.Data.IRepository;
using _7Colors.Models;

namespace _7Colors.Data.Repository
{
    public class ProductTypeRepository : Repository<ProductType>, IProductTypeRepository
    {
        private readonly AppDbContext _db;

        public ProductTypeRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}