using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ata.Investment.Allocation.Application;
using Ata.Investment.AuthCore;

namespace Ata.Investment.Api.Pages
{
    public class Seed : PageModel
    {
        private readonly UserSeed _userSeed;
        private readonly AllocationSeed _allocationSeed;

        public Seed(UserSeed userSeed, AllocationSeed allocationSeed)
        {
            _userSeed = userSeed;
            _allocationSeed = allocationSeed;
        }

        public async Task OnGet()
        {
            await _userSeed.Seed();
            // await _allocationSeed.SeedDraft();
        }
    }
}