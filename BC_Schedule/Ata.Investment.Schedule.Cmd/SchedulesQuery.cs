using MediatR;
using Ata.Investment.Profile.Domain;

namespace Ata.Investment.Schedule.Cmd;

public record SchedulesQuery(Guid AdvisorId, DateTimeOffset DateTime)
    : IRequest<IEnumerable<MeetingScheduleDTO>>;
