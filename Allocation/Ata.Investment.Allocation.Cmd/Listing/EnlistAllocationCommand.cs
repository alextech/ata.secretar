using MediatR;
using SharedKernel;

namespace Ata.Investment.Allocation.Cmd.Listing
{
    public class EnlistAllocationCommand : ICommand, ILoggable
    {
        public string LogDisplayName { get; } = "Allocation version activated";

        public int Version { get; }

        public EnlistAllocationCommand(int version)
        {
            Version = version;
        }
    }
}