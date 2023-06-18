using MediatR;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Schedule.Cmd;

namespace Ata.Investment.Schedule.Application;

public class UnprocessedMeetingsQueryHandler : IRequestHandler<UnprocessedMeetingsQuery, IEnumerable<UnprocessedMeetingDTO>>
{
    private readonly ProfileContext _profileContext;
    private readonly IMediator _mediatR;

    public UnprocessedMeetingsQueryHandler(ProfileContext profileContext, IMediator mediatR)
    {
        _profileContext = profileContext;
        _mediatR = mediatR;
    }

    public async Task<IEnumerable<UnprocessedMeetingDTO>> Handle(UnprocessedMeetingsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<UnprocessedMeetingDTO> processList = (
            from m in _profileContext.Meetings
                .AsNoTracking().TagWith("Fetching unprocessed meetings for Processing Team")
            where m.IsProcessed == false
                  && m.IsCompleted == true
            select new UnprocessedMeetingDTO
            {
                MeetingGuid = m.Guid,
                DateTime = m.Date,
                MeetingWith = m.Household.PrimaryClient.Name
            }
        );

        return processList;
    }
}