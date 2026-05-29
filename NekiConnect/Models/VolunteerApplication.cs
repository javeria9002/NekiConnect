namespace NekiConnect.Models
{
    // One row per volunteer application.
    // Volunteers apply to Campaigns (not Fundraisings — those are money-only).
    public class VolunteerApplication
    {
        public int Id { get; set; }

        // FK → ApplicationUser (the volunteer/donor)
        public string UserId { get; set; } = string.Empty;

        // FK → Campaign
        public int CampaignId { get; set; }

        // e.g. "teaching, first aid, driving"
        public string Skills { get; set; } = string.Empty;

        // e.g. "weekends only", "full day on Dec 15"
        public string Availability { get; set; } = string.Empty;

        // "Pending", "Accepted", "Rejected"
        public string Status { get; set; } = "Pending";

        // NGO marks this true after the campaign is completed
        public bool Attended { get; set; } = false;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser? User { get; set; }
        public Campaign? Campaign { get; set; }
    }
}