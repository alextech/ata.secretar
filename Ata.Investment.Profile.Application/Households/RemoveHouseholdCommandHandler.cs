using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Ata.Investment.Profile.Cmd.Households;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Application.Households
{
    public class RemoveHouseholdCommandHandler : IRequestHandler<RemoveHouseholdCommand, CommandResponse>
    {
        private readonly ProfileContext _profileContext;

        public RemoveHouseholdCommandHandler(ProfileContext profileContext)
        {
            _profileContext = profileContext;
        }

        public async Task<CommandResponse> Handle(RemoveHouseholdCommand householdCommand, CancellationToken cancellationToken)
        {
            Household household = await (
                from h in _profileContext.Households
                where h.Guid == householdCommand.HouseholdId
                select h
            )
                .Include(h => h.PrimaryClient)
                .Include(h => h.JointClient)
                .SingleAsync(cancellationToken);

            string clientName = household.PrimaryClient.Name;

            _profileContext.Households.Remove(household);

            _profileContext.Clients.Remove(household.PrimaryClient);
            if (household.IsJoint)
            {
                _profileContext.Clients.Remove(household.JointClient!);
            }

            IQueryable<Meeting> meetings =
                from m in _profileContext.Meetings
                where m.Household.Guid == householdCommand.HouseholdId
                select m;

            _profileContext.Meetings.RemoveRange(meetings);

            await _profileContext.SaveChangesAsync(cancellationToken);

            return CommandResponse.Ok(
                $"Household with {clientName} was permanently deleted."
            );
        }
    }
}