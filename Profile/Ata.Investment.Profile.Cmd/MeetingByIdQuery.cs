using System;
using MediatR;
using Ata.Investment.Profile.Domain;

namespace Ata.Investment.Profile.Cmd
{
    public class MeetingByIdQuery : IRequest<Meeting>
    {
        public Guid MeetingId { get; }

        public MeetingByIdQuery(Guid meetingId)
        {
            MeetingId = meetingId;
        }
    }
}