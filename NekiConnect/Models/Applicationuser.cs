using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NekiConnect.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Donor";

        public bool IsSuspended { get; set; } = false;

        public string? City { get; set; }

        public string? Bio { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}