using System;
using SharedKernel;

namespace Ata.Investment.Profile.Cmd.Households
{
    public class RestoreHouseholdCommand : ICommand, ILoggable
    {
        public string LogDisplayName { get; } = "Household restored";

        public Guid HouseholdId { get; }

        public RestoreHouseholdCommand(Guid householdId)
        {
            HouseholdId = householdId;
        }
    }
}