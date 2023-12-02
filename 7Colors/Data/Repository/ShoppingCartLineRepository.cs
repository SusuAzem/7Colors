using _7Colors.Data.IRepository;
using _7Colors.Models;

namespace _7Colors.Data.Repository
{
    public class ShoppingCartLineRepository : Repository<ShoppingCartLine>, IShoppingCartLineRepository
    {
        private readonly AppDbContext _db;

        public ShoppingCartLineRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public int DecrementCount(ShoppingCartLine shoppingCart, int count)
        {
            shoppingCart.Count -= count;
            return shoppingCart.Count;
        }

        public int IncrementCount(ShoppingCartLine shoppingCart, int count)
        {
            shoppingCart.Count += count;
            return shoppingCart.Count;
        }
    }
}
