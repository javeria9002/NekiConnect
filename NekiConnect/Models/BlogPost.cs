namespace NekiConnect.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        public int NgoId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        // ── ADDED: category for filtering and tagging ──
        public string Category { get; set; } = "Impact";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public NGO? NGO { get; set; }
    }
}
