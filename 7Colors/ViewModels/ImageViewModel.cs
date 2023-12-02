using _7Colors.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace _7Colors.ViewModels
{
    public class ImageViewModel
    {
        public int Id { get; set; }

        [Display(Name = "الصورة")]
        [ValidateNever]
        public string? Url { get; set; }

        [Required(ErrorMessage = "حقل الوصف مطلوب")]
        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "حقل العنوان مطلوب")]
        [Display(Name = "عنوان الصورة")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "حقل عرض الصورة مطلوب")]
        [Display(Name = "عرض الصورة؟")]
        public bool IsPublished { get; set; }

        [Required(ErrorMessage = "حقل الموضوع مطلوب")]
        [Display(Name = "الموضوع")]
        public int PostId { get; set; }
        public Post? Post { get; set; }
    }
}
