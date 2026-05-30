namespace NekiConnect.Models
{
    public class Donation
    {
        public int Id { get; set; }
        public string DonorId { get; set; } = string.Empty;
        public int? FundraisingId { get; set; }      // keep nullable — DB column stays
        public int? CampaignId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentReference { get; set; }
        public string PaymentMethod { get; set; } = "JazzCash";
        public DateTime DonatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties (only models that exist)
        public ApplicationUser? Donor { get; set; }
        public Campaign? Campaign { get; set; }
    }
}