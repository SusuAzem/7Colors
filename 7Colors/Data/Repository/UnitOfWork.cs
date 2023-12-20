using _7Colors.Data.IRepository;
using _7Colors.Models;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace _7Colors.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            ProductType = new ProductTypeRepository(_db);
            SpecialTag = new SpecialTagRepository(_db);
            Product = new ProductRepository(_db);
            ShoppingCartLine = new ShoppingCartLineRepository(_db);
            OrderItem = new OrderItemRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            User = new UserRepository(_db);
            Post = new PostRepository(_db);
            Image = new ImageRepository(_db);
        }
    
        public IProductTypeRepository ProductType { get; private set; }
        public ISpecialTagRepository SpecialTag { get; private set; }            
        public IProductRepository Product { get; private set; }
        public IShoppingCartLineRepository ShoppingCartLine { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderItemRepository OrderItem { get; private set; }
        public IUserRepository User { get; private set; }
        public IImageRepository Image { get; }
        public IPostRepository Post { get; }

        public async void Save()
        {
            await _db.SaveChangesAsync();
        }        
    }
}
