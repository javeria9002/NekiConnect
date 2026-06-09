using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NekiConnect.Interfaces;

namespace NekiConnect.Pages.Account
{
    [IgnoreAntiforgeryToken]
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;

        public LoginModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string Password { get; set; } = string.Empty;
        [BindProperty] public string? Role { get; set; }
        [BindProperty] public string? ReturnUrl { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var (ok, error, actualRole, token) =
                await _authService.LoginAsync(Email, Password);

            if (!ok)
            {
                var errorCode = error switch
                {
                    "wrongpassword" => "wrongpassword",
                    "suspended" => "suspended",
                    "ngo_pending" => "ngo_pending",    // ✅ added
                    "ngo_rejected" => "ngo_rejected",   // ✅ added
                    "ngo_not_found" => "ngo_not_found",  // ✅ added
                    "ngo_suspended" => "ngo_suspended",  // ✅ add 
                    _ => "invalid"
                };
                return Redirect($"/login?error={errorCode}&tab={Role?.ToLower() ?? "donor"}");
            }

            if (!string.IsNullOrEmpty(Role) &&
                !string.Equals(actualRole, Role, StringComparison.OrdinalIgnoreCase))
            {
                return Redirect($"/login?error=wrongtab&tab={Role!.ToLower()}");
            }

            Response.Cookies.Append("jwt", token!, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });

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