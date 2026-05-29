namespace NekiConnect.Models
{
    public class Donation
    {
        public int Id { get; set; }

        public string DonorId { get; set; } = string.Empty;

        public int? FundraisingId { get; set; }

        public int? CampaignId { get; set; }

        public decimal Amount { get; set; }

        public string? PaymentReference { get; set; }

        // ── ADDED ──
        public string PaymentMethod { get; set; } = "JazzCash";

        public DateTime DonatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser? Donor { get; set; }
        public Fundraising? Fundraising { get; set; }
        public Campaign? Campaign { get; set; }
    }
}