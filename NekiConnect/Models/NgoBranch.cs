using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NekiConnect.Models
{
    [Table("NgoBranches")]
    public class NgoBranch
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NgoId { get; set; }

        [Required]
        [StringLength(150)]
        public string BranchName { get; set; } = string.Empty;

        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Address { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [ForeignKey(nameof(NgoId))]
        public NGO? NGO { get; set; }
    }
}