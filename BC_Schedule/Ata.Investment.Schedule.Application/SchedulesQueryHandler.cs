using MediatR;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Schedule.Cmd;

namespace Ata.Investment.Schedule.Application;

public class SchedulesQueryHandler : IRequestHandler<SchedulesQuery, IEnumerable<MeetingScheduleDTO>>
{
    private readonly ProfileContext _profileContext;

    public SchedulesQueryHandler(ProfileContext profileContext)
    {
        _profileContext = profileContext;
    }

    public async Task<IEnumerable<MeetingScheduleDTO>> Handle(SchedulesQuery schedulesQuery, CancellationToken cancellationToken)
    {
        IEnumerable<MeetingScheduleDTO> schedule = (
            from m in _profileContext.Meetings
                .AsNoTracking().TagWith("Fetching MeetingScheduleDTO for a schedule list.")
            where m.Date.Date.Year == schedulesQuery.DateTime.Year
                && m.Date.Date.Month == schedulesQuery.DateTime.Month
                && m.Date.Date.Day == schedulesQuery.DateTime.Day
                && m.AdvisorGuid == schedulesQuery.AdvisorId
            select new MeetingScheduleDTO
            {
                MeetingGuid = m.Guid,
                DateTime = m.Date,
                MeetingWith = m.Household.PrimaryClient.Name,
                IsCompleted = m.IsCompleted
            }
        );

        return schedule;
    }
}