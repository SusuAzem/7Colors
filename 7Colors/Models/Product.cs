using System.ComponentModel.DataAnnotations.Schema;


namespace _7Colors.Models
{
    public class Product
    {     
        public int Id { get; set; }

        public string Name { get; set; }

        public float Price { get; set; }

        public string Image { get; set; }="/images/noimage.PNG";

        public string ProductColor { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsPublished { get; set; }

        [ForeignKey("ProductType")]
        public int TypeId { get; set; }
        [ForeignKey("TypeId")]
        public virtual ProductType? ProductType { get; set; }


        [ForeignKey("SpecialTag")]
        public int TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual SpecialTag? SpecialTag { get; set; }

    }
}
