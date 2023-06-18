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
    public class DraftAtVersionQueryHandler : IRequestHandler<DraftAtVersionQuery, VersionDraft?>
    {
        private readonly AllocationContext _allocationContext;

        public DraftAtVersionQueryHandler(AllocationContext allocationContext)
        {
            _allocationContext = allocationContext;
        }

        public async Task<VersionDraft?> Handle(DraftAtVersionQuery draftQuery, CancellationToken cancellationToken)
        {
            VersionDraft draft = await (
                // No Tracking version is useful for REST APIs where entity becomes disconnected
                // from d in _allocationContext.VersionDrafts.AsNoTracking()
                from d in _allocationContext.VersionDrafts
                where d.Version == draftQuery.Version
                select d
            ).FirstOrDefaultAsync(cancellationToken);

            return draft;
        }
    }
}