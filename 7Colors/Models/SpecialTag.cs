using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _7Colors.Models
{
    public class SpecialTag
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "حقل العلامة الخاصة مطلوب")]
        [Display(Name = "العلامة الخاصة - tag")]
        public string? Name { get; set; }
        [JsonIgnore]
        public virtual IEnumerable<Product> Products { get; set; } = new List<Product>();

    }
}
