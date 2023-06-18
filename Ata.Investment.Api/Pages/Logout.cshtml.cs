using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ata.Investment.AuthCore;

namespace Ata.Investment.Api.Pages
{
    public class Logout : PageModel
    {
        private readonly SignInManager<Advisor> _signInManager;

        public Logout(SignInManager<Advisor> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGet()
        {
            await _signInManager.SignOutAsync();
            // _logger.LogInformation("User logged out.");
            return Redirect("/login");
        }
    }
}