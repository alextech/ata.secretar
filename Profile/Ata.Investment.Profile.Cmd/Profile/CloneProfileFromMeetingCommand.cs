using System;
using SharedKernel;

namespace Ata.Investment.Profile.Cmd.Profile
{
    public class CloneProfileFromMeetingCommand : ICommand<Guid>, ILoggable
    {
        public string LogDisplayName { get; } = "Meeting started using previous data";

        public CloneProfileFromMeetingCommand(Guid meetingId)
        {
            MeetingId = meetingId;
        }

        public Guid MeetingId { get; }
    }
}