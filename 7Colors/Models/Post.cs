using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _7Colors.Models
{
    public class Post
    {
        public Post()
        {
            Images = new List<Image>();
            Created = DateTime.Now.Date;
        }
        public int Id { get; set; }
        
        public string? Title { get; set; }
   
        public string? Description { get; set; }

        public DateTime Created { get; set; } 

        public virtual IEnumerable<Image> Images { get; set; }   
    }
}
