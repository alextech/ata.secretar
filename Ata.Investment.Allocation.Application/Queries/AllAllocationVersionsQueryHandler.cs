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
    public class AllAllocationVersionsQueryHandler : IRequestHandler<AllAllocationVersionsQuery, IEnumerable<VersionDTO>>
    {
        private readonly AllocationContext _allocationContext;

        public AllAllocationVersionsQueryHandler(AllocationContext allocationContext)
        {
            _allocationContext = allocationContext;
        }
        public async Task<IEnumerable<VersionDTO>> Handle(AllAllocationVersionsQuery request, CancellationToken cancellationToken)
        {
            // emphasize that this is query definition, NOT query result
            IQueryable<int?> publishedVersionsQuery =
                from av in _allocationContext.AllocationVersions.TagWith("Published versions query.")
                group av by av.Version
                into published
                select (int?)published.Key;

            // query definitions are built up from other query definitions until ToList invocation

            IQueryable<VersionDTO> versionsQuery =
                from d in _allocationContext.VersionDrafts.TagWith("List all version drafts.")
                join av in publishedVersionsQuery
                    on d.Version equals av into publishedDrafts
                from p in publishedDrafts.DefaultIfEmpty()
                orderby d.Version descending
                select new VersionDTO()
                {
                    Version = d.Version,
                    Description = d.Description ?? "",
                    IsPublished = p != null
                };

            ListedAllocationVersionsQuery listedQuery = new ListedAllocationVersionsQuery();
            ListedAllocationVersionsQueryHandler listedQueryHandler = new ListedAllocationVersionsQueryHandler(_allocationContext);

            List<VersionDTO> allVersions = await versionsQuery.ToListAsync(cancellationToken: cancellationToken);
            List<int> listedVersions = (await listedQueryHandler.Handle(listedQuery, CancellationToken.None)).ToList();

            foreach (VersionDTO versionDTO in allVersions.Where(versionDTO => listedVersions.Contains(versionDTO.Version)))
            {
                versionDTO.IsListed = true;
            }

            return allVersions;
        }
    }
}