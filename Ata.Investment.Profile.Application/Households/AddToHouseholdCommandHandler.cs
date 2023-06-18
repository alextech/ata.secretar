using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Ata.Investment.Profile.Cmd.Households;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Application.Households
{
    public class AddToHouseholdCommandHandler : ICommandHandler<AddToHouseholdCommand, CommandResponse>
    {
        private readonly ProfileContext _profileContext;

        public AddToHouseholdCommandHandler(ProfileContext profileContext)
        {
            _profileContext = profileContext;
        }

        public async Task<CommandResponse> Handle(AddToHouseholdCommand addToHouseholdCommand, CancellationToken cancellationToken)
        {

            Household householdToUpdate = await (
                    from h in _profileContext.Households
                    where h.Guid == addToHouseholdCommand.HouseholdId
                    select h
                )
                .Include(h => h.PrimaryClient)
                .Include(h => h.JointClient)
            .SingleAsync(cancellationToken);

            householdToUpdate.AddMember(addToHouseholdCommand.Client);

            await _profileContext.SaveChangesAsync(cancellationToken);

            return CommandResponse.Ok(
                $"{addToHouseholdCommand.Client.Name} was added to household with {householdToUpdate.PrimaryClient.Name}."
            );
        }
    }
}