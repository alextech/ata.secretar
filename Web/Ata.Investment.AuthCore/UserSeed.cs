using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Ata.Investment.AuthCore
{
    public class UserSeed
    {
        private readonly UserManager<Advisor> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AuthDbContext _dbContext;

        public UserSeed(UserManager<Advisor> userManager, RoleManager<IdentityRole> roleManager, AuthDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task Seed()
        {
            bool roleExists = await _roleManager.RoleExistsAsync("Advisor");
            if (! roleExists)
            {
                IdentityRole advisorRole = new IdentityRole("Advisor");
                await _roleManager.CreateAsync(advisorRole);
            }

            await _dbContext.SaveChangesAsync();

            Advisor radion = await _userManager.FindByEmailAsync("ra_provotorov@example.com");
            if (radion == null)
            {
                radion = new Advisor
                {
                    Name = "Провоторов, Радион Александрович",
                    UserName = "ra_provotorov",
                    Email = "ra_provotorov@example.com",
                    EmailConfirmed = true,
                    Credentials = "CFP, BBA, BMath",
                    LockoutEnabled =  false
                };
                await _userManager.CreateAsync(radion, "%sD72#q1#QaI");
                await _dbContext.SaveChangesAsync();

                await _userManager.AddToRoleAsync(radion, "Advisor");
                await _dbContext.SaveChangesAsync();
            }

            Advisor anton = await _userManager.FindByEmailAsync("zaytsev_ad@example.com");
            if (anton == null)
            {
                anton = new Advisor
                {
                    Name = "Зайцев, Антон Дмитриевич",
                    UserName = "ad_zaytsev",
                    Email = "ad_zaytsev@example.com",
                    EmailConfirmed = true,
                    Credentials = "CFP, FMA, PFP",
                    LockoutEnabled = false
                };

                IdentityResult createResult = await _userManager.CreateAsync(anton, "nfFii%6G7XD6");
                await _dbContext.SaveChangesAsync();

                await _userManager.AddToRoleAsync(anton, "Advisor");
                await _dbContext.SaveChangesAsync();
            }

            Advisor evgeniy = await _userManager.FindByEmailAsync("ey_kozlov@example.com");
            if (evgeniy == null)
            {
                evgeniy = new Advisor
                {
                    Name = "Козлов, Евгений Ярославич",
                    UserName = "ey_kozlov",
                    Email = "ey_kozlov@example.com",
                    EmailConfirmed = true,
                    Credentials = "BA, PFP",
                    LockoutEnabled = false
                };

                IdentityResult createResult = await _userManager.CreateAsync(evgeniy, "69xrW#cK^TnM");
                await _dbContext.SaveChangesAsync();

                await _userManager.AddToRoleAsync(evgeniy, "Advisor");
                await _dbContext.SaveChangesAsync();
            }

            Advisor marina = await _userManager.FindByEmailAsync("ma_romanenko@example.com");
            if (marina == null)
            {
                marina = new Advisor
                {
                    Name = "Романенко, Марина Анатольевна",
                    UserName = "ma_romanenko",
                    Email = "ma_romanenko@example.com",
                    EmailConfirmed = true,
                    Credentials = "",
                    LockoutEnabled = false
                };

                IdentityResult createResult = await _userManager.CreateAsync(marina, "mxob(Xu6NHw8");
                await _dbContext.SaveChangesAsync();

                await _userManager.AddToRoleAsync(marina, "Advisor");
                await _dbContext.SaveChangesAsync();
            }

            await SeedStaff();
        }

        private async Task SeedStaff()
        {
            bool roleExists = await _roleManager.RoleExistsAsync("Staff");
            if (!roleExists)
            {
                IdentityRole staffRole = new IdentityRole("Staff");
                await _roleManager.CreateAsync(staffRole);
            }

            await _dbContext.SaveChangesAsync();

            Advisor varya = await _userManager.FindByEmailAsync("vi_morozova@example.com");
            if (varya == null)
            {
                varya = new Advisor()
                {
                    Name = "Морозова, Варья Ивановна",
                    UserName = "vi_morozova",
                    Email = "vi_morozova@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = false
                };

                await _userManager.CreateAsync(varya, "y#9kCi>&X;t9");
                await _dbContext.SaveChangesAsync();

                await _userManager.AddToRoleAsync(varya, "Staff");
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}