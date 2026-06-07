using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NekiConnect.Models
{
    [Table("Feedbacks")]
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NgoId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; } = 5;

        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ✅ Verified badge type
        public string VerifiedAs { get; set; } = string.Empty; // "Donor" or "Volunteer"

        [ForeignKey(nameof(NgoId))]
        public NGO? NGO { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }
    }
}