using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ata.Investment.AuthCore;

namespace Ata.Investment.Api.Pages
{
    // ReSharper disable once InconsistentNaming
    public class _Host : PageModel
    {
        private readonly UserManager<Advisor> _userManager;

        public Guid UserGuid { get; private set; }

        public _Host(UserManager<AuthCore.Advisor> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Redirect("/login");
            }

            UserGuid = (
                from u in _userManager.Users
                where u.UserName == HttpContext.User.Identity.Name
                select new Guid(u.Id)
            ).Single();

            return Page();
        }
    }
}