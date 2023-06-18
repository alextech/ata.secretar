using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharedKernel;
using Ata.Investment.Profile.Cmd.Profile;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain;

namespace Ata.Investment.Profile.Application.Profile
{
    public class ProcessMeetingCommandHandler : ICommandHandler<ProcessMeetingCommand, CommandResponse>
    {
        private readonly ProfileContext _profileContext;

        public ProcessMeetingCommandHandler(ProfileContext profileContext)
        {
            _profileContext = profileContext;
        }

        public async Task<CommandResponse> Handle(ProcessMeetingCommand processMeetingCommand, CancellationToken cancellationToken)
        {
            Meeting meeting = (
                from m in _profileContext.Meetings
                where m.Guid == processMeetingCommand.MeetingId
                select m
            ).First();

            meeting.Process();

            await _profileContext.SaveChangesAsync(cancellationToken);
            return CommandResponse.Ok(
                $"Meeting for \"{meeting.Purpose}\" was processed"
            );
        }
    }
}