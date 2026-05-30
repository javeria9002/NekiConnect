using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NekiConnect.Services;

namespace NekiConnect.Pages.Account
{
    // Handles the actual login form POST from Login.razor
    public class LoginModel : PageModel
    {
        private readonly AuthService _authService;

        public LoginModel(AuthService authService)
        {
            _authService = authService;
        }

        [BindProperty(SupportsGet = false)] public string Email { get; set; } = string.Empty;
        [BindProperty(SupportsGet = false)] public string Password { get; set; } = string.Empty;
        [BindProperty(SupportsGet = false)] public string? Role { get; set; }
        [BindProperty(SupportsGet = false)] public string? ReturnUrl { get; set; }
        [BindProperty(SupportsGet = false)] public bool RememberMe { get; set; } = true;

        public async Task<IActionResult> OnPostAsync()
        {
            // Try to login
            var (ok, error, actualRole) = await _authService.LoginAsync(Email, Password, RememberMe);

            if (!ok)
            {
                // Login failed → redirect back with error
                var errorCode = error?.Contains("password") == true ? "wrongpassword"
                              : error?.Contains("suspended") == true ? "suspended"
                              : "invalid";
                return Redirect($"/login?error={errorCode}&tab={Role?.ToLower() ?? "donor"}");
            }

            // Check that the user picked the right tab (Donor / NGO / Admin)
            if (!string.IsNullOrEmpty(Role) &&
                !string.Equals(actualRole, Role, StringComparison.OrdinalIgnoreCase))
            {
                await _authService.LogoutAsync();   // cookie already set — sign them out
                return Redirect($"/login?error=wrongtab&tab={Role.ToLower()}");
            }

            // Success → redirect to ReturnUrl OR to role's dashboard
            if (!string.IsNullOrEmpty(ReturnUrl))
                return Redirect(ReturnUrl);

            return actualRole switch
            {
                "Admin" => Redirect("/admin/dashboard"),
                "NGO" => Redirect("/ngo/dashboard"),
                _ => Redirect("/donor/dashboard")
            };
        }
    }
}