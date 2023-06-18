using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedTestingKernel;
using Ata.Investment.Allocation.Domain;
using Ata.Investment.Allocation.Domain.Composition;
using Xunit;
using Xunit.Abstractions;

namespace Ata.Investment.Allocation.Data.Test
{
    public class AllocationContextTest : IDisposable
    {
        public AllocationContextTest(ITestOutputHelper output)
        {
            _loggerFactory = DbLoggerFactory.CreateLoggerForOutput(output);
            _optionsBuilder.UseSqlServer(
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Ata.Investment.Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        [Fact]
        public async Task AllocationOptionsRetrieveTest()
        {
            using (AllocationContext context = new AllocationContext(_optionsBuilder.Options))
            {
                Domain.Allocation allocation = (from a in context.Allocations
                        where a.Name == "growth"
                        select a)
                    .First();

                var sampleOptions = SampleOptions;
                AllocationVersion allocationVersion = allocation.CreateVersion(1809)
                    .WithOptions(new List<AllocationOption>
                    {
                        sampleOptions[0],
                        sampleOptions[1],
                        sampleOptions[2]

                    });

                await context.SaveChangesAsync();
            }

            _optionsBuilder.UseLoggerFactory(_loggerFactory);
            using (AllocationContext context = new AllocationContext(_optionsBuilder.Options))
            {
                var allocations =
                    from a in context.AllocationVersions.AsNoTracking()
                        .Include(al => al.Options)
                    where a.Name == "growth"
                    orderby a.Version descending
                    select a;

                AllocationVersion allocation = allocations.First();

                Assert.Equal(3, allocation.Options.Count());

                //TODO fix false positive pass on invalid fundcode && percent combo
                Assert.True(
                    allocation.Options.Select(
                        o => o.CompositionParts.Select(cp => cp.FundCode == "CIG1111" && cp.Percent == 40)).Any());
                Assert.True(
                    allocation.Options.Select(
                        o => o.CompositionParts.Select(cp => cp.FundCode == "CIG2222" && cp.Percent == 60)).Any());

                Assert.True(
                    allocation.Options.Select(
                        o => o.CompositionParts.Select(cp => cp.FundCode == "PMO1111" && cp.Percent == 50)).Any());
                Assert.True(
                    allocation.Options.Select(
                        o => o.CompositionParts.Select(cp => cp.FundCode == "ABC2222" && cp.Percent == 30)).Any());
                Assert.True(
                    allocation.Options.Select(
                        o => o.CompositionParts.Select(cp => cp.FundCode == "PMO3333" && cp.Percent == 20)).Any());
                
                Assert.True(
                    context.Funds.Select(f => f.FundCode == "CIG1111").Any());
            }

            _optionsBuilder.UseLoggerFactory(null);
        }

        private readonly ILoggerFactory _loggerFactory;

        private readonly DbContextOptionsBuilder<AllocationContext> _optionsBuilder
            = new DbContextOptionsBuilder<AllocationContext>();

        private List<AllocationOption> SampleOptions
        {
            get
            {
                Option option = new Option("Stub Option _1");

                Option option2 = new Option("Stub Option _2");
                
                Option option3 = new Option("Stub Option _3");

                return new List<AllocationOption>
                {
                    option.CreateComposition(
                        new Dictionary<string, int>
                        {
                            {"CIG1111", 40},
                            {"CIG2222", 60}
                        }),
                    option2.CreateComposition(
                        new Dictionary<string, int>
                        {
                            {"PMO1111", 50},
                            {"ABC2222", 30},
                            {"PMO3333", 20}
                        }),
                    option3.CreateComposition(
                        new Dictionary<string, int>
                        {
                            {"CIG1111", 100}
                        })
                };
            }
        }
        
        public void Dispose()
        {
            _optionsBuilder.UseLoggerFactory(null);
            using (AllocationContext context = new AllocationContext(_optionsBuilder.Options))
            {
                context.Database.ExecuteSqlRaw("DELETE FROM [Funds].[CompositionPart]");
                context.Database.ExecuteSqlRaw("DELETE FROM [Funds].[AllocationOptions]");
                context.Database.ExecuteSqlRaw("DELETE FROM [Funds].[Option]");
                context.Database.ExecuteSqlRaw("DELETE FROM [Funds].[AllocationVersions]");
                context.Database.ExecuteSqlRaw("DELETE FROM [Funds].[Funds]");
            }
        }
    }
}