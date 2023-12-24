using _7Colors.Models;

namespace _7Colors.Data.IRepository
{
    public interface IUnitOfWork
    {
        ISpecialTagRepository SpecialTag { get; }
        IProductTypeRepository ProductType { get; }
        IProductRepository Product { get; }
        IShoppingCartLineRepository ShoppingCartLine { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderItemRepository OrderItem { get; }
        IUserRepository User { get; }
        IImageRepository Image { get; }
        IPostRepository Post { get; }

        Task Save();
    }
}
