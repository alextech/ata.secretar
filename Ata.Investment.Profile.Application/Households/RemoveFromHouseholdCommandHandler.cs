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
    public class RemoveFromHouseholdCommandHandler : IRequestHandler<RemoveFromHouseholdCommand, CommandResponse>
    {
        private readonly ProfileContext _profileContext;

        public RemoveFromHouseholdCommandHandler(ProfileContext profileContext)
        {
            _profileContext = profileContext;
        }

        public async Task<CommandResponse> Handle(RemoveFromHouseholdCommand removeCommand, CancellationToken cancellationToken)
        {
            Household householdToUpdate = await (
                    from h in _profileContext.Households
                    where h.Guid == removeCommand.HouseholdId
                    select h
                )
                .Include(h => h.PrimaryClient)
                .Include(h => h.JointClient)
                .SingleAsync(cancellationToken);

            householdToUpdate.RemoveMember(removeCommand.ClientId);

            string clientName = householdToUpdate.GetClientById(removeCommand.ClientId).Name;

            await _profileContext.SaveChangesAsync(cancellationToken);

            return CommandResponse.Ok(
                $"Client {clientName} was removed from household with {householdToUpdate.PrimaryClient.Name}."
            );
        }
    }
}