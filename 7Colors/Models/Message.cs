using System.ComponentModel.DataAnnotations;

namespace _7Colors.Models
{
    public class Message
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="حقل الاسم مطلوب")]
        [Display(Name = "الاسم")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "حقل البريد مطلوب")]
        [EmailAddress]
        [Display(Name = "البريد الالكتروني")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "حقل المحتوى مطلوب")]
        [Display(Name = "المحتوى")]
        public string? Content { get; set; }
    }
}
