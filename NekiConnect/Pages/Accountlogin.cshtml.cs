using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NekiConnect.Models;

namespace NekiConnect.Pages
{
    public class AccountLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string Password { get; set; } = string.Empty;
        [BindProperty] public string Role { get; set; } = string.Empty;
        [BindProperty] public string ReturnUrl { get; set; } = string.Empty;

        public IActionResult OnGet() => Redirect("/login");

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(Email.Trim().ToLower());

                if (user == null)
                    return Redirect($"/login?error=invalid&tab={Role.ToLower()}&returnUrl={ReturnUrl}");

                if (user.Role != Role)
                    return Redirect($"/login?error=wrongtab&tab={Role.ToLower()}&returnUrl={ReturnUrl}");

                if (user.IsSuspended)
                    return Redirect($"/login?error=suspended&tab={Role.ToLower()}&returnUrl={ReturnUrl}");

                var result = await _signInManager.PasswordSignInAsync(
                    user, Password, isPersistent: true, lockoutOnFailure: false);

                if (!result.Succeeded)
                    return Redirect($"/login?error=wrongpassword&tab={Role.ToLower()}&returnUrl={ReturnUrl}");

                // ✅ Use returnUrl if provided, otherwise fall back to role default
                var destination = !string.IsNullOrEmpty(ReturnUrl) && ReturnUrl.StartsWith("/")
                    ? ReturnUrl
                    : user.Role switch
                    {
                        "Admin" => "/admin",
                        "NGO" => "/ngo/dashboard",
                        _ => "/donor/dashboard"
                    };

                return Redirect(destination);
            }
            catch
            {
                return Redirect($"/login?error=invalid&returnUrl={ReturnUrl}");
            }
        }
    }
}
