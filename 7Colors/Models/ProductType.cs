using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations;

namespace _7Colors.Models
{
    public class ProductType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "حقل النوع مطلوب")]
        [Display(Name = "نوع المنتج")]
        public string? Type { get; set; }

        [JsonIgnore] 
        public virtual IEnumerable<Product>? Products { get; set; }

    }
}
