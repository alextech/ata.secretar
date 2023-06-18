using MediatR;
using Ata.Investment.Profile.Domain;

namespace Ata.Investment.Schedule.Cmd;

public record UnprocessedMeetingsQuery()
    : IRequest<IEnumerable<UnprocessedMeetingDTO>>;