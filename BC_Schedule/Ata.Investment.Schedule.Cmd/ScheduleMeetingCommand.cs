using SharedKernel;

namespace Ata.Investment.Schedule.Cmd;

public class ScheduleMeetingCommand : ICommand, ILoggable
{
    public Guid HouseholdId { get; }

    public Guid AdvisorGuid { get; }

    public DateTimeOffset DateTime { get; }

    public string LogDisplayName { get; } = "Meeting scheduled";

    public ScheduleMeetingCommand(Guid householdId, Guid advisorGuid, DateTimeOffset dateTime)
    {
        HouseholdId = householdId;
        AdvisorGuid = advisorGuid;
        DateTime = dateTime;
    }
}