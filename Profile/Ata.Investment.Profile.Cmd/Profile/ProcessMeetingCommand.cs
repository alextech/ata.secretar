using System;
using SharedKernel;

namespace Ata.Investment.Profile.Cmd.Profile
{
    public class ProcessMeetingCommand : ICommand, ILoggable
    {
        public string LogDisplayName { get; } = "Meeting ended";

        public Guid MeetingId { get; }

        public ProcessMeetingCommand(Guid meetingId)
        {
            MeetingId = meetingId;
        }
    }
}