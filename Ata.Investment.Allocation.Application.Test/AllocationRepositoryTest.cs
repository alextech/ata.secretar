using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SharedKernel;
using Ata.Investment.Allocation.Data;
using Ata.Investment.Allocation.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Ata.Investment.Allocation.Application.Test
{
    public class AllocationRepositoryTest
    {
        public AllocationRepositoryTest(ITestOutputHelper output)
        {
        }

        [Fact]
        public async Task NextVersionNumberGeneration_SimpleTest()
        {
            // using (AllocationContext draftContext = new AllocationContext(TestDbContextOptions<AllocationContext>()))
            // {
            //     // setup
            //
            //     VersionDraft draft_1 = new VersionDraft(1803);
            //     VersionDraft draft_2 = new VersionDraft(1809);
            //
            //     draftContext.VersionDrafts.Add(draft_1);
            //     draftContext.VersionDrafts.Add(draft_2);
            //
            //     draftContext.SaveChanges();
            //
            //     Mock<TimeProvider> timeMock = new Mock<TimeProvider>();
            //     timeMock.SetupGet(tp => tp.UtcNow).Returns(new DateTime(2018, 10, 1));
            //     TimeProvider.Current = timeMock.Object;
            //
            //     AllocationRepository allocationService = new AllocationRepository(draftContext);
            //
            //     // act
            //     VersionDraft version = await allocationService.GenerateNextVersionDraft();
            //
            //     // verify
            //     Assert.Equal(1810, version.Version);
            // }
        }

        [Fact]
        public async Task NextVersionNumberGeneration_CrossYearTest()
        {
            // using (AllocationContext draftContext = new AllocationContext(TestDbContextOptions<AllocationContext>()))
            // {
            //     // setup
            //
            //     VersionDraft draft_1 = new VersionDraft(1803);
            //     VersionDraft draft_2 = new VersionDraft(1812);
            //
            //     draftContext.VersionDrafts.Add(draft_1);
            //     draftContext.VersionDrafts.Add(draft_2);
            //
            //     draftContext.SaveChanges();
            //
            //     Mock<TimeProvider> timeMock = new Mock<TimeProvider>();
            //     timeMock.SetupGet(tp => tp.UtcNow).Returns(new DateTime(2019, 1, 15));
            //     TimeProvider.Current = timeMock.Object;
            //
            //     AllocationRepository allocationService = new AllocationRepository(draftContext);
            //
            //     // act
            //     VersionDraft version = await allocationService.GenerateNextVersionDraft();
            //
            //     // verify
            //     Assert.Equal(1901, version.Version);
            // }
        }

        [Fact]
        public async Task NextVersionNumberGeneration_FrequentCreationTest()
        {
            // using (AllocationContext draftContext = new AllocationContext(TestDbContextOptions<AllocationContext>()))
            // {
            //     // setup
            //
            //     VersionDraft draft_1 = new VersionDraft(1903);
            //     VersionDraft draft_2 = new VersionDraft(1902);
            //
            //     draftContext.VersionDrafts.Add(draft_1);
            //     draftContext.VersionDrafts.Add(draft_2);
            //
            //     draftContext.SaveChanges();
            //
            //     Mock<TimeProvider> timeMock;
            //     VersionDraft versionDraft;
            //
            //     timeMock = new Mock<TimeProvider>();
            //     timeMock.SetupGet(tp => tp.UtcNow).Returns(new DateTime(2019, 3, 15));
            //     TimeProvider.Current = timeMock.Object;
            //
            //     AllocationRepository allocationService = new AllocationRepository(draftContext);
            //
            //     // act + verify
            //     versionDraft = await allocationService.GenerateNextVersionDraft();
            //     Assert.Equal(190315, versionDraft.Version);
            //
            //     draftContext.VersionDrafts.Add(versionDraft);
            //     draftContext.SaveChanges();
            //     //====
            //
            //     timeMock = new Mock<TimeProvider>();
            //     timeMock.SetupGet(tp => tp.UtcNow).Returns(new DateTime(2019, 3, 15, 9, 0, 0));
            //     TimeProvider.Current = timeMock.Object;
            //
            //     versionDraft = await allocationService.GenerateNextVersionDraft();
            //     Assert.Equal(19031509, versionDraft.Version);
            //
            //     draftContext.VersionDrafts.Add(versionDraft);
            //     draftContext.SaveChanges();
            //
            //     // ====
            //     timeMock = new Mock<TimeProvider>();
            //     timeMock.SetupGet(tp => tp.UtcNow).Returns(new DateTime(2019, 3, 15, 9, 5, 0));
            //     TimeProvider.Current = timeMock.Object;
            //
            //     versionDraft = await allocationService.GenerateNextVersionDraft();
            //     Assert.Equal(1903150905, versionDraft.Version);
            //
            //     draftContext.VersionDrafts.Add(versionDraft);
            //
            //     // ====
            // }
        }

        [Fact]
        private async Task NextVersionClonedFromExisting()
        {
            // using (AllocationContext draftContext =
            //     new AllocationContext(TestDbContextOptions<AllocationContext>()))
            // {
            //     VersionDraft draft = new VersionDraft(1703) {Draft = "draft xml"};
            //
            //     draftContext.Add(draft);
            //     draftContext.SaveChanges();
            //
            //     Mock<TimeProvider> timeMock = new Mock<TimeProvider>();
            //     timeMock.SetupGet(tp => tp.UtcNow).Returns(new DateTime(2018, 03, 1));
            //     TimeProvider.Current = timeMock.Object;
            //
            //     AllocationRepository allocationService = new AllocationRepository(draftContext);
            //     VersionDraft newDraft = await allocationService.CloneFrom(1703);
            //
            //     Assert.Equal(1803, newDraft.Version);
            //     Assert.Equal("draft xml", newDraft.Draft);
            //
            //     int savedNewDraft = (
            //         from d in draftContext.VersionDrafts.AsNoTracking()
            //         where d.Version == 1803
            //         select d
            //     ).Count();
            //
            //     Assert.Equal(1, savedNewDraft);
            // }
        }

        [Fact]
        private async Task FetchPublishedVersionNumbersTest()
        {
            using (AllocationContext context =
                new AllocationContext(TestDbContextOptions<AllocationContext>()))
            {
                Allocation.Domain.Allocation allocation = new Allocation.Domain.Allocation("safety", 1);
                context.Add(allocation);
                
                allocation.CreateVersion(1903);
                allocation.CreateVersion(1904);

                context.SaveChanges();
                
                AllocationRepository allocationService = new AllocationRepository(context);

                IEnumerable<int> versions = (await allocationService.FetchPublishedVersionNumbers()).ToList();
                
                Assert.Equal(1904, versions.ElementAt(0));
                Assert.Equal(1903, versions.ElementAt(1));
            }
        }

        private static DbContextOptions<T> TestDbContextOptions<T>() where T : DbContext
        {
            // Create a new service provider to create a new in-memory database.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance using an in-memory database and 
            // IServiceProvider that the context should resolve all of its 
            // services from.
            var builder = new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase(string.Concat(typeof(T), "-MemoryDb"))
                .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }
}