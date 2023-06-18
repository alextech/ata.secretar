using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Profile.Cmd.Advisors;
using Ata.Investment.Profile.Domain;

namespace Ata.Investment.Profile.Application.Advisors
{
    public class AdvisorByIdQueryHandler : IRequestHandler<AdvisorByIdQuery, Advisor>
    {
        private readonly UserManager<AuthCore.Advisor> _userManager;

        public AdvisorByIdQueryHandler(UserManager<AuthCore.Advisor> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Advisor> Handle(AdvisorByIdQuery request, CancellationToken cancellationToken)
        {
            Advisor advisor = (
                from advisorAuth in (await _userManager.Users.Where(u => u.Id == request.AdvisorId.ToString())
                    .TagWith("Fetching advisor by ID.").ToListAsync(cancellationToken))
                select new Advisor(
                    Guid.Parse(advisorAuth.Id),
                    advisorAuth.Name,
                    advisorAuth.Credentials,
                    advisorAuth.Email)).Single();

            return advisor;
        }
    }
}