using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace _7Colors.Models
{
    public class User
    {
        public User()
        {
        }
        public User(string nameidentifier, string name, string givenName, string surname, string email)
        {
            NameIdentifier = nameidentifier;
            Name = name;
            GivenName = givenName;
            Surname = surname;
            Email = email;
            Age = 3;
            ParentEmail = "-";
            Phone = "05";
            ParentPhone = "05";
            StreetAddress = "-";
            City = "-";
            Neighborhood = "-";
            PostalCode = 10000;
            Role = "User";
            Registered = false;
            Orders = new List<OrderHeader>();
            LockoutEnd =  new DateTimeOffset();
        }

        [Key]
        public string NameIdentifier { get; set; }

        public string Name { get; set; }

        public string GivenName { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string ParentEmail { get; set; }

        public string ParentPhone { get; set; }

        public string StreetAddress { get; set; }

        public string Neighborhood { get; set; }

        public string City { get; set; }

        public int PostalCode { get; set; }

        public int Age { get; set; }

        public string Role { get; set; }

        public bool Registered { get; set; }

        public DateTimeOffset LockoutEnd { get; set; }

        public virtual IEnumerable<OrderHeader> Orders { get; set; }
    }
}
