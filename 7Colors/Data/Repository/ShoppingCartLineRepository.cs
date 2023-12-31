﻿using _7Colors.Data.IRepository;
using _7Colors.Models;

using Microsoft.EntityFrameworkCore;

namespace _7Colors.Data.Repository
{
    public class ShoppingCartLineRepository : Repository<ShoppingCartLine>, IShoppingCartLineRepository
    {
        private readonly AppDbContext db;

        public ShoppingCartLineRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }
        public int DecrementCount(ShoppingCartLine shoppingCart, int count)
        {
            shoppingCart.Count -= count;
            return shoppingCart.Count;
        }

        public IEnumerable<ShoppingCartLine> GetList(string id)
        {
            return db.ShoppingCartLines.Where(s => s.UserNameIdentifier == id)
                .Include(s => s.Product)
                .ThenInclude(p => p!.ProductType)
                .ToList();
        }

        public int IncrementCount(ShoppingCartLine shoppingCart, int count)
        {
            shoppingCart.Count += count;
            return shoppingCart.Count;
        }
    }
}
