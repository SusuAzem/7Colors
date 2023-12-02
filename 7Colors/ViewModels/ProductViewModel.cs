using _7Colors.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _7Colors.ViewModels
{
    public class ProductViewModel
    {
        public Product? Product { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? ProductTypeList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? SpecialTagList { get; set; }
    }
}
