using MediatR;
using SharedKernel;
using Ata.Investment.Allocation.Domain;

namespace Ata.Investment.Allocation.Cmd
{
    public class CloneAllocationCommand : ICommand<VersionDraft>, ILoggable
    {
        public string LogDisplayName { get; } = "Allocation version cloned";
        public int Version { get; }

        public CloneAllocationCommand(int version)
        {
            Version = version;
        }
    }
}