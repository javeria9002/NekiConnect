using Microsoft.AspNetCore.Identity;
using NekiConnect.Models;
using NekiConnect.Interfaces;

namespace NekiConnect.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;  

        public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService)  // ✅ interface
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<(bool ok, string? error, string? role, string? token)> LoginAsync(
            string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Email and password required", null, null);

            var user = await _userManager.FindByEmailAsync(email.Trim().ToLower());

            if (user == null)
                return (false, "invalid", null, null);

            if (user.IsSuspended)
                return (false, "suspended", null, null);

            var valid = await _userManager.CheckPasswordAsync(user, password);

            if (!valid)
                return (false, "wrongpassword", null, null);

            // ✅ GET ROLE FROM IDENTITY SYSTEM
            var roles = await _userManager.GetRolesAsync(user);

            string role;

            if (roles == null || roles.Count == 0)
            {
                role = "Donor";

                // optional: assign default role properly
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                role = roles.First();
            }

            // ⚠️ optional sync field (not required for auth)
            user.Role = role;
            await _userManager.UpdateAsync(user);

            var token = _tokenService.GenerateToken(user);

            return (true, null, role, token);
        }
    }
}