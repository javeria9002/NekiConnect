using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NekiConnect.Models
{
    [Table("BlogPosts")]
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NgoId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(5000)]
        public string Content { get; set; } = string.Empty;

        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(50)]
        public string Category { get; set; } = "General";

        // "Draft", "Published"
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Published";

        public int ViewCount { get; set; } = 0;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(NgoId))]
        public NGO? NGO { get; set; }
    }
}