using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace _7Colors.Models
{
    public class Image
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "حقل الصورة مطلوب")]
        [Display(Name = "الصورة")]
        public string? Url { get; set; } = "";

        [Required(ErrorMessage = "حقل الوصف مطلوب")]
        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "حقل العنوان مطلوب")]
        [Display(Name = "عنوان الصورة")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "حقل عرض الصورة مطلوب")]
        [Display(Name = "عرض الصورة؟")]
        public bool IsPublished { get; set; }

        [Required(ErrorMessage = "حقل المجموعة مطلوب")]
        [Display(Name = "المجموعة")]
        [ForeignKey("Group")]
        public int GroupId { get; set; }

        [ForeignKey("GroupId")]
        public virtual HPGroup Group { get; set; } = null!;
    }
}
