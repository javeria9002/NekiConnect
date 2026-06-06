using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NekiConnect.Models
{
    [Table("VolunteerApplications")]
    public class VolunteerApplication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public int? CampaignId { get; set; }
        public int? EventId { get; set; }

        [StringLength(300)]
        public string Skills { get; set; } = string.Empty;

        [StringLength(300)]
        public string Availability { get; set; } = string.Empty;

        // "Pending", "Accepted", "Rejected"
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public bool Attended { get; set; } = false;

        [Required]
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [ForeignKey(nameof(CampaignId))]
        public Campaign? Campaign { get; set; }


        [ForeignKey(nameof(EventId))]
        public Event? Event { get; set; }
    }
}