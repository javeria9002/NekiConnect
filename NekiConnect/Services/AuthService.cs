using Microsoft.AspNetCore.Identity;
using NekiConnect.Models;

namespace NekiConnect.Services
{
    // Handles login/logout for the Blazor pages (cookie-based).
    // For JWT API auth, see AuthApiController instead.
    public class AuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // Returns: (success, errorMessage, role)
        public async Task<(bool ok, string? error, string? role)> LoginAsync(
            string email, string password, bool rememberMe = false)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Email and password are required.", null);

            var user = await _userManager.FindByEmailAsync(email.Trim().ToLower());
            if (user == null)
                return (false, "No account found with this email.", null);

            if (user.IsSuspended)
                return (false, "Your account has been suspended. Contact support.", null);

            var result = await _signInManager.PasswordSignInAsync(
                user, password, isPersistent: rememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
                return (false, "Incorrect password. Please try again.", null);

            return (true, null, user.Role);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}