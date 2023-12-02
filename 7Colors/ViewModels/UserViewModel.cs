using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace _7Colors.ViewModels
{
    public class UserViewModel
    {
        [Required]
        [Display(Name = " المعرف")]
        public string? Nameidentifier { get; set; }

        [Required(ErrorMessage = "حقل الاسم مطلوب")]
        [Display(Name = "الاسم")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "حقل الاسم الأول مطلوب")]
        [Display(Name = "الاسم الأول")]
        public string? GivenName { get; set; }

        [Required(ErrorMessage = "حقل اسم العائلة مطلوب")]
        [Display(Name = "اسم العائلة")]
        public string? Surname { get; set; }

        [Required(ErrorMessage = "حقل البريد الالكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "ليست صيغة صحيحة للبريد الاكتروني ")]
        [Display(Name = " البريد الالكتروني")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "حقل الرقم مطلوب")]
        [Display(Name = "الرقم")]
        [RegularExpression("/^(05)([0-9]{8})$/", ErrorMessage = "الرقم يبدأ ب 05")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "حقل البريد الالكتروني لولي الأمر مطلوب")]
        [Remote("NotEqualEmail", "Account", AdditionalFields = "Email", 
            ErrorMessage = "بريد ولي الأمر يجب أن لا يطابق بريد المستخدم", HttpMethod = "post")]
        [EmailAddress(ErrorMessage = "ليست صيغة صحيحة للبريد الاكتروني ")]
        [Display(Name = " بريد لولي الأمر")]
        public string? ParentEmail { get; set; }

        [Required(ErrorMessage = "حقل رقم ولي الأمر مطلوب")]
        [Display(Name = "رقم ولي الأمر")]        
        [Remote("NotEqualPhone", "Account", AdditionalFields = "Phone",
            ErrorMessage = "رقم ولي الأمر يجب أن لا يطابق رقم المستخدم", HttpMethod = "post")]
        [RegularExpression("/^(05)([0-9]{8})$/", ErrorMessage = "الرقم يبدأ ب 05")]
        public string? ParentPhone { get; set; }

        [Required(ErrorMessage = "حقل الشارع مطلوب")]
        [Display(Name = "الشارع")]
        public string? StreetAddress { get; set; }

        [Required(ErrorMessage = "حقل الحي مطلوب")]
        [Display(Name = "الحي")]
        public string? Neighborhood { get; set; }

        [Required(ErrorMessage = "حقل المدينة مطلوب")]
        [Display(Name = "المدينة")]
        public string? City { get; set; }
       
        [Display(Name = "الرمز البريدي")]
        [Range(10000, 99999)]
        public int PostalCode { get; set; }

        [Required(ErrorMessage = "حقل العمر مطلوب")]
        [Display(Name = "العمر")]
        [Range(3, 100 , ErrorMessage ="العمر من 3 وما فوق")]
        public int Age { get; set; }

        [Display(Name = "الدور")]
        [ValidateNever]
        public string? Role { get; set; }

        [ValidateNever]
        public bool Registered { get; set; }

        [ValidateNever]
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
