using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NekiConnect.Models
{
    [Table("Events")]
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NgoId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [StringLength(150)]
        public string? Location { get; set; }

        public DateTime EventDate { get; set; } = DateTime.Today;

        public int SeatsAvailable { get; set; }

        public int RegisteredCount { get; set; } = 0;

        [StringLength(20)]
        public string Status { get; set; } = "Upcoming";   // Upcoming / Completed / Cancelled

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(NgoId))]
        public NGO? NGO { get; set; }
    }
}