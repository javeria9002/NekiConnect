using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NekiConnect.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        // "Donor", "NGO", "Admin"
        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Donor";

        public bool IsSuspended { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}