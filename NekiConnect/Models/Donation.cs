namespace NekiConnect.Models
{
    public class Donation
    {
        public int Id { get; set; }
        public string DonorId { get; set; }
        public int CampaignId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DonatedAt { get; set; }
    }
}