using System;
using SharedKernel;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Cmd.Households
{
    public class AddToHouseholdCommand : ICommand, ILoggable
    {
        public string LogDisplayName { get; } = "Client added to household";

        public Guid HouseholdId { get; }
        public Client Client { get; }

        public AddToHouseholdCommand(Guid householdId, Client client)
        {
            HouseholdId = householdId;
            Client = client;
        }
    }
}