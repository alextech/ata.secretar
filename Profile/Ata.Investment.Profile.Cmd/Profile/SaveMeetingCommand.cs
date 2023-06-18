using SharedKernel;
using Ata.Investment.Profile.Domain;

namespace Ata.Investment.Profile.Cmd.Profile
{
    public class SaveMeetingCommand : ICommand
    {
        public Meeting Meeting { get; }

        public SaveMeetingCommand(Meeting meeting)
        {
            Meeting = meeting;
        }
    }
}