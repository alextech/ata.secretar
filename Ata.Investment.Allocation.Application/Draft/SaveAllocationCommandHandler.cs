using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Allocation.Cmd.Draft;
using Ata.Investment.Allocation.Data;
using Ata.Investment.Allocation.Domain;

namespace Ata.Investment.Allocation.Application.Draft
{
    public class SaveAllocationCommandHandler : IRequestHandler<SaveAllocationCommand, Unit>
    {
        private readonly AllocationContext _allocationContext;

        public SaveAllocationCommandHandler(AllocationContext allocationContext)
        {
            _allocationContext = allocationContext;
        }

        public async Task<Unit> Handle(SaveAllocationCommand saveCommand, CancellationToken cancellationToken)
        {
            VersionDraft versionDraft = await (
                from vd in _allocationContext.VersionDrafts
                where vd.Version == saveCommand.VersionDraft.Version
                select vd
            ).FirstOrDefaultAsync(cancellationToken);

            Mapper.Map<VersionDraft, VersionDraft>(saveCommand.VersionDraft, versionDraft);

            _allocationContext.Update(versionDraft);
            await _allocationContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}