using System;
using System.Threading;
using System.Threading.Tasks;
using SharedKernel;
using Ata.Investment.Profile.Cmd.Profile;
using Ata.Investment.Profile.Domain;

namespace Ata.Investment.Profile.Application.Profile
{
    public class SaveMeetingCommandHandler : ICommandHandler<SaveMeetingCommand, CommandResponse>
    {
        private readonly MeetingRepository _meetingRepository;

        public SaveMeetingCommandHandler(MeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        public async Task<CommandResponse> Handle(SaveMeetingCommand saveMeetingCommand, CancellationToken cancellationToken)
        {
            Meeting meetingToUpdate = await _meetingRepository.FindByIdAsync(saveMeetingCommand.Meeting.Guid);
            if (meetingToUpdate.IsCompleted)
            {
                throw new Exception("Attempting to save meeting that is completed.");
            }

            meetingToUpdate.PopulateFrom(saveMeetingCommand.Meeting);

            _meetingRepository.Update(meetingToUpdate);
            await _meetingRepository.FlushAsync(cancellationToken);

            return CommandResponse.Ok(
                $"Meeting {saveMeetingCommand.Meeting.Guid} saved."
            );
        }
    }
}