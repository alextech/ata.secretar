using System;
using Newtonsoft.Json;
using SharedKernel;
using Ata.Investment.Profile.Domain;

namespace Ata.Investment.Profile.Cmd.Profile
{
    public class BeginMeetingCommand : ICommand<Guid>, ILoggable
    {
        public string LogDisplayName { get; } = "Begin new meeting";

        public Guid HouseholdId { get; }
        public Advisor Advisor { get; }
        public int AllocationVersion { get; }

        public BeginMeetingCommand(Guid householdId, Advisor advisor, int allocationVersion)
        {
            HouseholdId = householdId;
            Advisor = advisor;
            AllocationVersion = allocationVersion;
        }

        [JsonConstructor]
        public BeginMeetingCommand(Guid householdId, int allocationVersion)
        {
            HouseholdId = householdId;
            AllocationVersion = allocationVersion;
        }
    }
}