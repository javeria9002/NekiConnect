using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NekiConnect.Models
{
    [Table("NGOs")]
    public class NGO
    {
        [Key]
        public int Id { get; set; }

        // FK → ApplicationUser
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Mission { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [StringLength(300)]
        public string LogoUrl { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string ContactEmail { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string ContactPhone { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string RegistrationNumber { get; set; } = string.Empty;

        [StringLength(500)]
        public string CertificateUrl { get; set; } = string.Empty;

        // "Pending", "Approved", "Rejected", "Suspended"
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [StringLength(500)]
        public string? RejectionReason { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
        public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    }
}