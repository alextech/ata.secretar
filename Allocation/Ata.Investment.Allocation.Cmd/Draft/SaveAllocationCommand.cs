using MediatR;
using Ata.Investment.Allocation.Domain;

namespace Ata.Investment.Allocation.Cmd.Draft
{
    public class SaveAllocationCommand : IRequest
    {
        public VersionDraft VersionDraft { get; }

        public SaveAllocationCommand(VersionDraft versionDraft)
        {
            VersionDraft = versionDraft;
        }
    }
}