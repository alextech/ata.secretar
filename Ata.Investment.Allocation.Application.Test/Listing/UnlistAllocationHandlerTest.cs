using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Allocation.Application.Listing;
using Ata.Investment.Allocation.Application.Queries;
using Ata.Investment.Allocation.Cmd;
using Ata.Investment.Allocation.Cmd.Listing;
using Ata.Investment.Allocation.Data;
using Xunit;

namespace Ata.Investment.Allocation.Application.Test.Listing
{
    public class UnlistAllocationHandlerTest
    {
        [Fact]
        public async Task ListingAllocationTest()
        {
            DbContextOptions<AllocationContext> options = new DbContextOptionsBuilder<AllocationContext>()
                .UseInMemoryDatabase("allocations_test_db")
                .Options;

            // ======== SETUP ===========
            await using (AllocationContext context = new AllocationContext(options))
            {
                Domain.Allocation allocation = new Domain.Allocation("safety", 1);
                context.Add(allocation);

                allocation.CreateVersion(1903);
                allocation.CreateVersion(1904);

                context.SaveChanges();
            }

            // ======== ACT 1 - ASSERT Listed by default ======
            await using (AllocationContext context = new AllocationContext(options))
            {

                ListedAllocationVersionsQuery query = new ListedAllocationVersionsQuery();
                ListedAllocationVersionsQueryHandler handler = new ListedAllocationVersionsQueryHandler(context);

                IEnumerable<int> versions = (await handler.Handle(query, CancellationToken.None)).ToList();

                Assert.Contains(1903, versions);
                Assert.Contains(1904, versions);
            }

            // ======== ACT 2 - UNLIST ===============
            await using (AllocationContext context = new AllocationContext(options))
            {
                UnlistAllocationCommand cmd = new UnlistAllocationCommand(1903);
                UnlistAllocationCommandHandler commandHandler = new UnlistAllocationCommandHandler(context);

                bool unlisted = (await commandHandler.Handle(cmd, CancellationToken.None)).Success;

                Assert.True(unlisted);
            }

            // ======== ASSERT unlisted ==============
            await using (AllocationContext context = new AllocationContext(options))
            {
                ListedAllocationVersionsQuery query = new ListedAllocationVersionsQuery();
                ListedAllocationVersionsQueryHandler handler = new ListedAllocationVersionsQueryHandler(context);

                IEnumerable<int> versions = (await handler.Handle(query, CancellationToken.None)).ToList();

                Assert.DoesNotContain(1903, versions);
                Assert.Contains(1904, versions);
            }

            // ====== ACT 3 - EnList ==================
            await using (AllocationContext context = new AllocationContext(options))
            {
                EnlistAllocationCommand cmd = new EnlistAllocationCommand(1903);
                EnlistAllocationCommandHandler commandHandler = new EnlistAllocationCommandHandler(context);

                bool unlisted = (await commandHandler.Handle(cmd, CancellationToken.None)).Success;

                Assert.True(unlisted);
            }

            // ======== ASSERT enlisted ==============
            await using (AllocationContext context = new AllocationContext(options))
            {
                ListedAllocationVersionsQuery query = new ListedAllocationVersionsQuery();
                ListedAllocationVersionsQueryHandler handler = new ListedAllocationVersionsQueryHandler(context);

                IEnumerable<int> versions = (await handler.Handle(query, CancellationToken.None)).ToList();

                Assert.Contains(1903, versions);
                Assert.Contains(1904, versions);
            }
        }
    }
}