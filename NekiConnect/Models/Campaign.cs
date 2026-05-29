namespace NekiConnect.Models
{
    // A Campaign has a physical location + date.
    // People can BOTH donate money to it AND apply to volunteer for it.
    public class Campaign
    {
        public int Id { get; set; }

        // FK → NGO
        public int NgoId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        // Human-readable address shown on the page
        public string Location { get; set; } = string.Empty;

        // Used for Google Maps pin
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DateTime CampaignDate { get; set; }

        // Campaign's own fundraising goal (separate from Fundraising module)
        public decimal GoalAmount { get; set; } = 0;

        // Updated automatically after every donation
        public decimal RaisedAmount { get; set; } = 0;

        // Max number of volunteers accepted
        public int SeatsAvailable { get; set; }

        // "Upcoming", "Completed", "Cancelled"
        public string Status { get; set; } = "Upcoming";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public NGO? NGO { get; set; }
        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
        public ICollection<VolunteerApplication> VolunteerApplications { get; set; } = new List<VolunteerApplication>();
    }
}