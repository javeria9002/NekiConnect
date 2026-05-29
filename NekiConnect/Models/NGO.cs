namespace NekiConnect.Models
{
    public class NGO
    {
        public int Id { get; set; }

        // FK → ApplicationUser
        public string UserId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Mission { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        // "Education", "Health / Medical", "Food & Nutrition", "Disaster Relief",
        // "Women Empowerment", "Child Welfare", "Environment", "Other"
        public string Category { get; set; } = string.Empty;

        public string LogoUrl { get; set; } = string.Empty;

        public string ContactEmail { get; set; } = string.Empty;

        public string ContactPhone { get; set; } = string.Empty;

        public string RegistrationNumber { get; set; } = string.Empty;

        // Path/URL to uploaded certificate document
        public string CertificateUrl { get; set; } = string.Empty;

        // "Pending", "Approved", "Rejected"
        public string Status { get; set; } = "Pending";

        // Admin fills this on rejection
        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Fundraising> Fundraisings { get; set; } = new List<Fundraising>();
        public ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
        public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
        public ICollection<BeneficiaryRecord> BeneficiaryRecords { get; set; } = new List<BeneficiaryRecord>();
    }
}