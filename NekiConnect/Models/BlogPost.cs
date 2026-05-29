namespace NekiConnect.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        public int NgoId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string Category { get; set; } = "Impact";

        // ── ADDED ──
        public string Status { get; set; } = "Published"; // "Published" or "Draft"
        public int ViewCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public NGO? NGO { get; set; }
    }
}