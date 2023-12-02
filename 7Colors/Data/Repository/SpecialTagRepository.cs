using _7Colors.Data.IRepository;
using _7Colors.Models;

namespace _7Colors.Data.Repository
{
    internal class SpecialTagRepository : Repository<SpecialTag>, ISpecialTagRepository
    {
        private readonly AppDbContext _db;

        public SpecialTagRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }     
    }
}