using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Allocation.Cmd;
using Ata.Investment.Allocation.Data;

namespace Ata.Investment.Allocation.Application.Queries
{
    public class ListedAllocationVersionsQueryHandler : IRequestHandler<ListedAllocationVersionsQuery, IEnumerable<int>>
    {
        private readonly AllocationContext _allocationContext;

        public ListedAllocationVersionsQueryHandler(AllocationContext allocationContext)
        {
            _allocationContext = allocationContext;
        }

        public async Task<IEnumerable<int>> Handle(ListedAllocationVersionsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<int> versions = await (
                from av in _allocationContext.AllocationVersions
                    .AsNoTracking().TagWith("Query listed allocation versions.")
                where av.IsListed == true
                group av by av.Version
                into published
                orderby published.Key descending
                select published.Key
            ).ToListAsync(cancellationToken: cancellationToken);

            return versions;
        }
    }
}