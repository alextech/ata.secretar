using System;
using SharedKernel;

namespace Ata.Investment.Profile.Cmd.Households
{
    public class RemoveFromHouseholdCommand : ICommand, ILoggable
    {
        public string LogDisplayName { get; } = "Member removed from household";

        public Guid HouseholdId { get; }
        public Guid ClientId { get; }

        public RemoveFromHouseholdCommand(Guid householdId, Guid clientId)
        {
            HouseholdId = householdId;
            ClientId = clientId;
        }
    }
}