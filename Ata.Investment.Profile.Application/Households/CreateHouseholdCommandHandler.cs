using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SharedKernel;
using Ata.Investment.Profile.Cmd.Households;
using Ata.Investment.Profile.Data;

namespace Ata.Investment.Profile.Application.Households
{
    public class CreateHouseholdCommandHandler : IRequestHandler<CreateHouseholdCommand, CommandResponse>
    {
        private readonly ProfileContext _profileContext;

        public CreateHouseholdCommandHandler(ProfileContext profileContext)
        {
            _profileContext = profileContext;
        }

        public async Task<CommandResponse> Handle(CreateHouseholdCommand request, CancellationToken cancellationToken)
        {
            await _profileContext.Households.AddAsync(request.Household, cancellationToken);
            await _profileContext.SaveChangesAsync(cancellationToken);

            return CommandResponse.Ok(
                $"Created household with {request.Household.PrimaryClient.Name}."
            );
        }
    }
}