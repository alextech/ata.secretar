using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Ata.Investment.Profile.Cmd.Households;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Application.Households
{
    public class RestoreHouseholdCommandHandler : ICommandHandler<RestoreHouseholdCommand, CommandResponse>
    {
        private readonly ProfileContext _profileContext;

        public RestoreHouseholdCommandHandler(ProfileContext profileContext)
        {
            _profileContext = profileContext;
        }

        public async Task<CommandResponse> Handle(RestoreHouseholdCommand householdCommand, CancellationToken cancellationToken)
        {
            Household household = await (
                from h in _profileContext.Households
                where h.Guid == householdCommand.HouseholdId
                select h
            )
                .Include(h => h.PrimaryClient)
                .SingleAsync(cancellationToken);

            household.Restore();

            await _profileContext.SaveChangesAsync(cancellationToken);

            return CommandResponse.Ok(
                $"Household with {household.PrimaryClient.Name} was restored."
            );
        }
    }
}