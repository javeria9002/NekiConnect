using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NekiConnect.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // ✅ Delete the JWT cookie
            Response.Cookies.Delete("jwt");
            return Redirect("/login");
        }

        public IActionResult OnPost()
        {
            Response.Cookies.Delete("jwt");
            return Redirect("/login");
        }
    }
}