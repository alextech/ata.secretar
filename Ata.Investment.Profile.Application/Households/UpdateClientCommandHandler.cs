using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Ata.Investment.Profile.Cmd.Households;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Application.Households
{
    public class UpdateClientCommandHandler : ICommandHandler<UpdateClientCommand, CommandResponse>
    {
        private readonly ProfileContext _profileContext;

        public UpdateClientCommandHandler(ProfileContext profileContext)
        {
            _profileContext = profileContext;
        }

        public async Task<CommandResponse> Handle(UpdateClientCommand updateCommand, CancellationToken cancellationToken)
        {
            Client clientToUpdate = await (
                from c in _profileContext.Clients
                where c.Guid == updateCommand.ClientId
                select c
            ).SingleAsync(cancellationToken);

            Mapper.Map(updateCommand.ClientBody, clientToUpdate);

            await _profileContext.SaveChangesAsync(cancellationToken);

            return CommandResponse.Ok(
                $"Personal information for {clientToUpdate.Name} was updated."
            );
        }
    }
}