using Microsoft.AspNetCore.Identity;

namespace NekiConnect.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string FullName { get; set; } = string.Empty;

		// "Donor", "NGO", "Admin"
		public string Role { get; set; } = "Donor";

		public bool IsSuspended { get; set; } = false;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}