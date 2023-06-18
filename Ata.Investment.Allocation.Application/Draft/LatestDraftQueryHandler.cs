using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Allocation.Cmd.Draft;
using Ata.Investment.Allocation.Data;
using Ata.Investment.Allocation.Domain;

namespace Ata.Investment.Allocation.Application.Draft
{
    public class LatestDraftQueryHandler : IRequestHandler<LatestDraftQuery, VersionDraft>
    {
        private readonly AllocationContext _allocationContext;

        public LatestDraftQueryHandler(AllocationContext allocationContext)
        {
            _allocationContext = allocationContext;
        }

        public async Task<VersionDraft> Handle(LatestDraftQuery request, CancellationToken cancellationToken)
        {
            VersionDraft latestDraft = await (
                // No Tracking version is useful for REST APIs where entity becomes disconnected
                // from d in _allocationContext.VersionDrafts.AsNoTracking()
                from d in _allocationContext.VersionDrafts
                orderby d.Date descending
                select d
            ).FirstAsync(cancellationToken);

            return latestDraft;
        }
    }
}