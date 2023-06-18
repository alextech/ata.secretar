using System;
using SharedKernel;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Cmd.Households
{
    public class UpdateClientCommand : ICommand, ILoggable
    {
        public string LogDisplayName { get; } = "Client details updated";

        public Guid ClientId { get; }
        public Client ClientBody { get; }

        public UpdateClientCommand(Guid clientId, Client clientBody)
        {
            ClientId = clientId;
            ClientBody = clientBody;
        }
    }
}