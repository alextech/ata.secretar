using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Profile.Cmd;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain;

namespace Ata.Investment.Profile.Application.Profile
{
    public class MeetingsQueryHandler : IRequestHandler<MeetingsQuery, IEnumerable<Meeting>>
    {
        private readonly ProfileContext _profileContext;

        public MeetingsQueryHandler(ProfileContext profileContext)
        {
            _profileContext = profileContext;
        }

        public async Task<IEnumerable<Meeting>> Handle(MeetingsQuery meetingsQuery, CancellationToken cancellationToken)
        {
            IQueryable<Meeting> q =
                from m in _profileContext.Meetings
                    .AsNoTracking().TagWith("Listing all meetings for household.")
                where m.Household.Guid == meetingsQuery.HouseholdId
                orderby m.Date descending
                select m;

            return await q.ToListAsync(cancellationToken);
        }
    }
}