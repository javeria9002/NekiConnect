using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NekiConnect.Models
{
    [Table("Campaigns")]
    public class Campaign
    {
        [Key]
        public int Id { get; set; }

        // FK → NGO
        [Required]
        public int NgoId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [Required]
        public DateTime CampaignDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GoalAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal RaisedAmount { get; set; } = 0;

        public int SeatsAvailable { get; set; }

        // "Upcoming", "Completed", "Cancelled"
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Upcoming";

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(NgoId))]
        public NGO? NGO { get; set; }

        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
        public ICollection<VolunteerApplication> VolunteerApplications { get; set; } = new List<VolunteerApplication>();
    }
}