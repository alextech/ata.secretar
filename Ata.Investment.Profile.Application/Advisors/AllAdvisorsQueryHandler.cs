using System;
using System.Collections.Generic;
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
    public class AllAdvisorsQueryHandler : IRequestHandler<AllAdvisorsQuery, IEnumerable<Advisor>>
    {
        private readonly UserManager<AuthCore.Advisor> _userManager;

        public AllAdvisorsQueryHandler(UserManager<AuthCore.Advisor> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<Advisor>> Handle(AllAdvisorsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Advisor> advisors =
                from advisorAuth in _userManager.GetUsersInRoleAsync("Advisor").Result
                select new Advisor(
                    Guid.Parse(advisorAuth.Id),
                    advisorAuth.Name,
                    advisorAuth.Credentials,
                    advisorAuth.Email
                );

            return advisors;
        }
    }
}