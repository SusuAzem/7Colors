using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Net;
using _7Colors.Models;

namespace _7Colors.ViewModels
{
    public class ProductItemViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="حقل الاسم مطلوب")]
        [Display(Name = "الاسم")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "حقل السعر مطلوب")] 
        [Display(Name = "السعر")]
        public float Price { get; set; }

        [ValidateNever]
        [Display(Name = "الصورة")]
        public string? Image { get; set; } = "";

        [Required(ErrorMessage = "حقل اللون مطلوب")]
        [Display(Name = "اللون")]
        public string? ProductColor { get; set; }

        [Required(ErrorMessage = "حقل توفر المنتج مطلوب")]
        [Display(Name = "متوفر؟")]
        public bool IsAvailable { get; set; }

        [Required(ErrorMessage = "حقل عرض المنتج مطلوب")]
        [Display(Name = "عرض المنتج؟")]
        public bool IsPublished { get; set; }

        [Required(ErrorMessage ="حقل نوع المنتج مطلوب")]
        [Display(Name = "نوع المنتج")]
        public int TypeId { get; set; }
        public ProductType? ProductType { get; set; }

        [Required(ErrorMessage = "حقل العلامة الخاصة مطلوب")]
        [Display(Name = "العلامة الخاصة")]
        public int TagId { get; set; }
        public SpecialTag? SpecialTag { get; set; }
    
}
}
