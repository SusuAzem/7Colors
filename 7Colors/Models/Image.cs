using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace _7Colors.Models
{
    public class Image
    {
        public int Id { get; set; }

        public string? Url { get; set; } = "-";

        public string? Description { get; set; }

        public string? Title { get; set; }

        public bool IsPublished { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public virtual Post? Post { get; set; }
    }
}
