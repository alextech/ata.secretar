using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharedKernel;
using Ata.Investment.Profile.Cmd.Profile;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain;

namespace Ata.Investment.Profile.Application.Profile
{
    public class CompleteMeetingCommandHandler : ICommandHandler<CompleteMeetingCommand, CommandResponse>
    {
        private readonly ProfileContext _profileContext;

        public CompleteMeetingCommandHandler(ProfileContext profileContext)
        {
            _profileContext = profileContext;
        }

        public async Task<CommandResponse> Handle(CompleteMeetingCommand completeCommand, CancellationToken cancellationToken)
        {
            Meeting meeting = (
                from m in _profileContext.Meetings
                where m.Guid == completeCommand.MeetingId
                select m
            ).First();

            meeting.Complete();

            await _profileContext.SaveChangesAsync(cancellationToken);
            return CommandResponse.Ok(
                $"Meeting for \"{meeting.Purpose}\" was completed and locked"
            );
        }
    }
}