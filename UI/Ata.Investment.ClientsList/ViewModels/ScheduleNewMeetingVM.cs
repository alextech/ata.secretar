using System;

namespace Ata.Investment.ClientsList.ViewModels;

public class ScheduleNewMeetingVM
{
    public Guid AdvisorId { get; set; }
    public Guid HouseholdId { get; set; }
    public DateTimeOffset DateTime { get; set; } = DateTimeOffset.Now;
}