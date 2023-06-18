using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Profile.Cmd;
using Ata.Investment.Profile.Cmd.Households;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Application.Households
{
    public class HouseholdByClientIdQueryHandler : IRequestHandler<HouseholdByClientIdQuery, Household>
    {
        private readonly ProfileContext _profileContext;

        public HouseholdByClientIdQueryHandler(ProfileContext profileContext)
        {
            _profileContext = profileContext;
        }
        public async Task<Household> Handle(HouseholdByClientIdQuery householdQuery, CancellationToken cancellationToken)
        {
            IQueryable<Household> q = (from h in _profileContext.Households
                    where h.PrimaryClient.Guid == householdQuery.ClientId || h.JointClient.Guid == householdQuery.ClientId
                    select h
                )
                .Include(h => h.PrimaryClient)
                .Include(h => h.JointClient);

            return await q.SingleOrDefaultAsync(cancellationToken);
        }
    }
}