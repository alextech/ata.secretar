using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Ata.Investment.Allocation.Cmd.Listing;
using Ata.Investment.Allocation.Data;

namespace Ata.Investment.Allocation.Application.Listing
{
    public class UnlistAllocationCommandHandler : IRequestHandler<UnlistAllocationCommand, CommandResponse>
    {
        private readonly AllocationContext _allocationContext;

        public UnlistAllocationCommandHandler(AllocationContext allocationContext)
        {
            _allocationContext = allocationContext;
        }

        public async Task<CommandResponse> Handle(UnlistAllocationCommand unlistCommand, CancellationToken cancellationToken)
        {
            await (
                from av in _allocationContext.AllocationVersions
                where av.Version == unlistCommand.Version
                select av
            ).ForEachAsync(av => av.IsListed = false, cancellationToken);

            await _allocationContext.SaveChangesAsync(cancellationToken);

            return CommandResponse.Ok(
                $"Allocation version {unlistCommand.Version} was deactivated."
            );
        }
    }
}