using System;

namespace Ata.Investment.Profile.Domain;

public class MeetingScheduleDTO
{
    public Guid MeetingGuid { get; init; }
    public string MeetingWith { get; init; }
    public DateTimeOffset DateTime { get; init; }
    public bool IsCompleted { get; init; }
}