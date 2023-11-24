using Microsoft.AspNetCore.Mvc.Rendering;

namespace _7Colors.Utility
{
    public static class Strings
    {
        public const string Role_User = "Student";
        public const string Role_Admin = "Admin";
        public const string Role_Teacher = "Teacher";
        public static string somethingWentWrong = "حصل خطأ ما ، حاول مرة أخرى";
        public static int success_code = 1;
        public static int failure_code = 0;

        public static List<SelectListItem> GetRolesForDropDown(bool isAdmin)
        {
            if (isAdmin)
            {
                return new List<SelectListItem>
                {
                     new SelectListItem{Value=Role_Admin, Text = Role_Admin}
                };
            }
            else
            {
                return new List<SelectListItem>
                {
                     new SelectListItem{Value = Role_User, Text = Role_User},
                     new SelectListItem{ Value = Role_Teacher, Text=Role_Teacher}
                };
            }
        }

        public static List<SelectListItem> GetTimeDropDown()
        {
            int minute = 60;
            List<SelectListItem> duration = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                duration.Add(new SelectListItem { Value = minute.ToString(), Text = i + " Hr" });
                minute = minute + 30;
                duration.Add(new SelectListItem { Value = minute.ToString(), Text = i + " Hr 30 m" });
                minute = minute + 30;
            }
            return duration;
        }
    }
}
