using MediatR;
using Ata.Investment.Allocation.Domain;

namespace Ata.Investment.Allocation.Cmd.Draft
{
    public class DraftAtVersionQuery : IRequest<VersionDraft?>
    {
        public int Version { get; }

        public DraftAtVersionQuery(int version)
        {
            Version = version;
        }
    }
}