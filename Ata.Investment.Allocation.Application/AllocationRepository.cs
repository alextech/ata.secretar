using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SharedKernel;
using Ata.Investment.Allocation.Data;
using Ata.Investment.Allocation.Domain;

namespace Ata.Investment.Allocation.Application
{
    public class AllocationRepository : GenericRepository<Domain.Allocation, AllocationContext>
    {
        private readonly AllocationContext _allocationContext;
    
        public AllocationRepository(AllocationContext draftContext) : base(draftContext)
        {
            _allocationContext = draftContext;
        }

        [Obsolete("Use ListedAllocationVersionsQuery")]
        public async Task<IEnumerable<int>> FetchPublishedVersionNumbers()
        {
            var versions = await (
                from av in _allocationContext.AllocationVersions
                group av by av.Version
                into published
                orderby published.Key descending 
                select published.Key
            ).ToListAsync();

            return versions;
        }

        public ValueTask<EntityEntry<AllocationVersion>> AddAsync(AllocationVersion allocationVersion)
        {
            return _allocationContext.AddAsync(allocationVersion);
        }

        public ValueTask<EntityEntry<VersionDraft>> AddAsync(VersionDraft allocationVersion)
        {
            return _allocationContext.AddAsync(allocationVersion);
        }
    }
}