namespace NekiConnect.Models
{
    public class NGO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Mission { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string CertificateUrl { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties (only models that exist)
        public ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
        public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    }
}