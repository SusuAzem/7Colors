using _7Colors.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _7Colors.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "حقل الاسم مطلوب")]
        [Display(Name = "عنوان الموضوع")]
        public string? Title { get; set; }

        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        public IEnumerable<Image> Images { get; set; }= new List<Image>();

        [Display(Name = "أنشأ بتاريخ ")]
        public DateTime Created { get; set; } = DateTime.Now.Date;
    }
}
