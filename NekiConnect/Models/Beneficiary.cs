using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NekiConnect.Models
{
    [Table("Beneficiaries")]
    public class Beneficiary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NgoId { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string Province { get; set; } = string.Empty;

        [StringLength(50)]
        public string AidType { get; set; } = "Food Aid";

        [StringLength(20)]
        public string Status { get; set; } = "Active";   // Active / Completed

        public string? Notes { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(NgoId))]
        public NGO? NGO { get; set; }
    }
}