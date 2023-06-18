using System;
using SharedKernel;

namespace Ata.Investment.Profile.Cmd.Profile
{
    public class EmailMeetingCommand : ICommand, ILoggable
    {
        public string LogDisplayName { get; } = "Emailed meeting PDF results";

        public Guid MeetingId { get; }

        public EmailMeetingCommand(Guid meetingId)
        {
            MeetingId = meetingId;
        }
    }
}