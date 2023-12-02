using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _7Colors.Models
{
    public class ShoppingCartLine
    {
        public int Id { get; set; }


        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }


        [Display(Name = "الكمية")]
        [Range(1, 1000, ErrorMessage = "الرجاء إدخال رقم بين 1 و 1000")]
        public int Count { get; set; }


        [ForeignKey("User")]
        public string? UserNameIdentifier { get; set; }
        [ForeignKey("UserNameIdentifier")]
        [ValidateNever]
        public virtual User? User { get; set; }


        [Display(Name = "السعر")]
        [NotMapped]
        public float LinePrice { get; set; }
    }
}
