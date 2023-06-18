using MediatR;
using Ata.Investment.Allocation.Domain;

namespace Ata.Investment.Allocation.Cmd.Draft
{
    public class LatestDraftQuery : IRequest<VersionDraft>
    {

    }
}