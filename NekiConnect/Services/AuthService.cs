using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Interfaces;
using NekiConnect.Models;

namespace NekiConnect.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IDbContextFactory<ApplicationDbContext> _factory; // ✅ added

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IDbContextFactory<ApplicationDbContext> factory) // ✅ added
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _factory = factory; // ✅ added
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

            var roles = await _userManager.GetRolesAsync(user);
            string role;

            if (roles == null || roles.Count == 0)
            {
                role = "Donor";
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                role = roles.First();
            }

            // ✅ NGO status check — only added this block
            if (role == "NGO")
            {
                await using var db = await _factory.CreateDbContextAsync();
                var ngo = await db.NGOs.FirstOrDefaultAsync(n => n.UserId == user.Id);

                if (ngo is null)
                    return (false, "ngo_not_found", null, null);

                if (ngo.Status == "Pending")
                    return (false, "ngo_pending", null, null);

                if (ngo.Status == "Rejected")
                    return (false, "ngo_rejected", null, null);

                if (ngo.Status == "Suspended")
                    return (false, "ngo_suspended", null, null);  // ✅ specific message
            }

            user.Role = role;
            await _userManager.UpdateAsync(user);

            var token = _tokenService.GenerateToken(user);
            return (true, null, role, token);
        }
    }
}