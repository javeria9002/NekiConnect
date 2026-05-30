using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NekiConnect.Models
{
    [Table("Donations")]
    public class Donation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DonorId { get; set; } = string.Empty;

        public int? FundraisingId { get; set; }   // kept for future
        public int? CampaignId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(100)]
        public string? PaymentReference { get; set; }

        [Required]
        [StringLength(30)]
        public string PaymentMethod { get; set; } = "JazzCash";

        [Required]
        public DateTime DonatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(DonorId))]
        public ApplicationUser? Donor { get; set; }

        [ForeignKey(nameof(CampaignId))]
        public Campaign? Campaign { get; set; }
    }
}