using System.Collections.Generic;
using MediatR;

namespace Ata.Investment.Allocation.Cmd
{
    public class AllAllocationVersionsQuery : IRequest<IEnumerable<VersionDTO>>
    {
        
    }
}