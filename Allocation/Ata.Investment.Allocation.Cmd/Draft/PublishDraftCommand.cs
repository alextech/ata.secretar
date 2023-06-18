using MediatR;
using SharedKernel;

namespace Ata.Investment.Allocation.Cmd.Draft
{
    public class PublishDraftCommand : ICommand, ILoggable
    {
        public string LogDisplayName { get; } = "Published allocation version";

        public PublishDraftCommand(int version)
        {
            Version = version;
        }

        public int Version { get; }
    }
}