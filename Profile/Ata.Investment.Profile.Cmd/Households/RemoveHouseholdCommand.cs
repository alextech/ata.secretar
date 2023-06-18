using System;
using SharedKernel;

namespace Ata.Investment.Profile.Cmd.Households
{
    public class RemoveHouseholdCommand : ICommand, ILoggable
    {
        public string LogDisplayName { get; } = "Household permanently removed";
        public Guid HouseholdId { get; }

        public RemoveHouseholdCommand(Guid householdId)
        {
            HouseholdId = householdId;
        }
    }
}