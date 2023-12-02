using _7Colors.Models;

namespace _7Colors.ViewModels
{
    public class ShoppingCartViewModel
    {
        public OrderHeader? OrderHeader { get; set; }
        public IEnumerable<ShoppingCartLine>? ListCart { get; set; }

}
}
