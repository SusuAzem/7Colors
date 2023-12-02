using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _7Colors.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [ForeignKey("OrderHeader")]
        [Required]
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public virtual OrderHeader? OrderHeader { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        public int Count { get; set; }

        public float ItemsPrice { get; set; }
    }
}
