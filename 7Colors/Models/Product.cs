using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Net;
using Stripe;

namespace _7Colors.Models
{
    public class Product
    {
        public Product()
        {
        }

        public Product(string name, float price, string productColor, bool isAvailable, bool isPublished, int typeId, int tagId)
        {
            Name = name;
            Price = price;
            ProductColor = productColor;
            IsAvailable = isAvailable;
            IsPublished = isPublished;
            TypeId = typeId;
            TagId = tagId;
            Image = "/images/noimage.PNG";
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public float Price { get; set; }

        public string Image { get; set; }

        public string ProductColor { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsPublished { get; set; }

        [ForeignKey("ProductType")]
        [Required]
        public int TypeId { get; set; }
        [ForeignKey("TypeId")]
        public virtual ProductType? ProductType { get; set; }


        [ForeignKey("SpecialTag")]
        [Required]
        public int TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual SpecialTag? SpecialTag { get; set; }

    }
}
