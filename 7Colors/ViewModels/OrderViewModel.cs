using _7Colors.Models;

namespace _7Colors.ViewModels
{
    public class OrderViewModel
    {
        public OrderHeader? OrderHeader { get; set; }
        public IEnumerable<OrderItem>? OrderItems { get; set; }
    }
}
