using MediatR;
using SharedKernel;

namespace Ata.Investment.Allocation.Cmd.Listing
{
    public class UnlistAllocationCommand : ICommand, ILoggable
    {
        public string LogDisplayName { get; } = "Allocation version deactivated";

        public int Version { get; }

        public UnlistAllocationCommand(int version)
        {
            Version = version;
        }
    }
}