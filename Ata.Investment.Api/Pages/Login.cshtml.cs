using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ata.Investment.AuthCore;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Ata.Investment.Api.Pages
{

    public class LoginInput
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Referer { get; set; }
    }

    public class Login : PageModel
    {
        private readonly UserManager<Advisor> _userManager;
        private readonly SignInManager<Advisor> _signInManager;

        public Login(UserManager<Advisor> userManager, SignInManager<Advisor> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public LoginInput Input { get; set; }

        [ViewData]
        public bool IsLoggedIn { get; set; }

        [ViewData]
        public string Referer { get; set; }

        public void OnGet([FromQuery(Name = "referer")] string referer)
        {
            IsLoggedIn = User.Identity?.IsAuthenticated ?? false;

            Referer = referer;
        }

        public async Task<IActionResult> OnPost()
        {
            SignInResult result = await _signInManager.PasswordSignInAsync(userName:Input.Username, password: Input.Password,
                isPersistent:false, lockoutOnFailure:false);

            string uri = Input.Referer ?? "/clients";

            if (result.Succeeded)
            {
                return Redirect(uri);
            }

            IsLoggedIn = false;

            return Page();
        }
    }
}