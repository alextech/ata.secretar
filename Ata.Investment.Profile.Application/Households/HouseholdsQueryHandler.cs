using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Profile.Cmd.Households;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Application.Households
{
    public class HouseholdsQueryHandler : IRequestHandler<HouseholdsQuery, IEnumerable<Household>>
    {
        private readonly ProfileContext _profileContext;

        public HouseholdsQueryHandler(ProfileContext profileContext)
        {
            _profileContext = profileContext;
        }

        public async Task<IEnumerable<Household>> Handle(HouseholdsQuery householdsQuery, CancellationToken cancellationToken)
        {
            IQueryable<Household> q = (
                    from h in _profileContext.Households
                    where h.IsActive == householdsQuery.IsActive || h.IsActive != householdsQuery.IsArchived
                    select h
                )
                .Include(h => h.PrimaryClient)
                .Include(h => h.JointClient);

            if (!string.IsNullOrEmpty(householdsQuery.ClientName))
            {
                q = q.Where(h =>
                    h.PrimaryClient.Name.Contains(householdsQuery.ClientName) ||
                    h.JointClient.Name.Contains(householdsQuery.ClientName));
            }


            return await q.ToListAsync(cancellationToken);
        }
    }
}