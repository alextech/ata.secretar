using System;
using SharedKernel;

namespace Ata.Investment.Profile.Cmd.Households
{
    public class ArchiveHouseholdCommand : ICommand, ILoggable
    {
        public Guid HouseholdId { get; }

        public string LogDisplayName { get; } = "Household archived";

        public ArchiveHouseholdCommand(Guid householdId)
        {
            HouseholdId = householdId;
        }
    }
}