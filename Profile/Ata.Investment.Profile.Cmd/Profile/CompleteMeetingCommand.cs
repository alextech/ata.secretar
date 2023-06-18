using System;
using SharedKernel;

namespace Ata.Investment.Profile.Cmd.Profile
{
    public class CompleteMeetingCommand : ICommand, ILoggable
    {
        public string LogDisplayName { get; } = "Meeting completed";

        public Guid MeetingId { get; }

        public CompleteMeetingCommand(Guid meetingId)
        {
            MeetingId = meetingId;
        }
    }
}