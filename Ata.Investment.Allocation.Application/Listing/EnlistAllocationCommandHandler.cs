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
    public class EnlistAllocationCommandHandler : IRequestHandler<EnlistAllocationCommand, CommandResponse>
    {
        private readonly AllocationContext _allocationContext;

        public EnlistAllocationCommandHandler(AllocationContext allocationContext)
        {
            _allocationContext = allocationContext;
        }

        public async Task<CommandResponse> Handle(EnlistAllocationCommand enlistCommand, CancellationToken cancellationToken)
        {
            await (
                from av in _allocationContext.AllocationVersions
                where av.Version == enlistCommand.Version
                select av
            ).ForEachAsync(av => av.IsListed = true, cancellationToken: cancellationToken);

            await _allocationContext.SaveChangesAsync(cancellationToken);

            return CommandResponse.Ok(
                $"Allocation version {enlistCommand.Version} was activated."
            );
        }
    }
}