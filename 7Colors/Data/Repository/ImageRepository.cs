using _7Colors.Data.IRepository;
using _7Colors.Models;

namespace _7Colors.Data.Repository
{
    public class ImageRepository : Repository<Image>, IImageRepository
    {
        private readonly AppDbContext _db;

        public ImageRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
