using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.Household;
using Ata.Investment.Profile.Domain.KYC;
using Ata.Investment.Profile.Domain.Profile;
using Xunit;

namespace Ata.Investment.Profile.Application.Test
{
    public class HouseholdRepositoryTest
    {
        [Fact]
        public void FetchByClientIdTest()
        {
            // DbContextOptions<ProfileContext> options = new DbContextOptionsBuilder<ProfileContext>()
            //     .UseInMemoryDatabase(databaseName: "household_repository_test")
            //     .Options;
            //
            //
            // Client c1 = new Client("p1", "p1@test", DateTimeOffset.MinValue);
            //
            // Client c2 = new Client("p2", "p2@test", DateTimeOffset.MinValue);
            // Household h = new Household(c1, c2);
            // using (ProfileContext context = new ProfileContext(options))
            // {
            //     context.Add(h);
            //     context.SaveChanges();
            //
            //     HouseholdRepository householdRepository = new HouseholdRepository(context);
            //     Household found = householdRepository.FetchByClientId(c1.Guid);
            //     Assert.Same(h, found);
            // }
        }

        [Fact]
        public void FetchAllHouseholdsTest()
        {
            // DbContextOptions<ProfileContext> options = new DbContextOptionsBuilder<ProfileContext>()
            //     .UseInMemoryDatabase(databaseName: "household_repository_test")
            //     .Options;
            //
            // Guid h1_id,  h2_id;
            // Client c1, c2, c3;
            //
            // using (ProfileContext context = new ProfileContext(options))
            // {
            //     c1 = new Client("cl_1", "cl_1@service.test", DateTimeOffset.Now);
            //     c2 = new Client("cl_2", "cl_2@service.test", DateTimeOffset.Now.Subtract(TimeSpan.FromDays(1)));
            //     c3 = new Client("cl_3", "cl_3@service.test", DateTimeOffset.Now);
            //
            //     Household h1 = new Household(c1, c2);
            //     Household h2 = new Household(c3);
            //
            //     h1_id = h1.Guid;
            //     h2_id = h2.Guid;
            //
            //     context.Add(h1);
            //     context.Add(h2);
            //     context.SaveChanges();
            // }
            //
            // using (ProfileContext context = new ProfileContext(options))
            // {
            //     HouseholdRepository householdRepository = new HouseholdRepository(context);
            //
            //     IEnumerable<Household> households = householdRepository.ListAll();
            //
            //     Assert.Equal(2, households.Count());
            //
            //     Household h1 = households.FirstOrDefault(h => h.Guid == h1_id);
            //     Household h2 = households.FirstOrDefault(h => h.Guid == h2_id);
            //
            //     Assert.NotNull(h1);
            //     Assert.NotNull(h2);
            //     Assert.NotNull(h1.PrimaryClient);
            //     Assert.Equal(c1.Guid, h1.PrimaryClient.Guid);
            //     Assert.Equal(c2.Guid, h1.JointClient.Guid);
            //     Assert.Equal(c3.Guid, h2.PrimaryClient.Guid);
            // }
        }

        // TODO find why does not work with inmemory database
        public void FetchProfilesInHousehold()
        {
            // profiles belong to household (regardless of who ever existed in it) (no navigation from profile to house)
            // since using lazy loading proxy, getting profiles from household
            // indirectly becomes repository's responsibility
            // DbContextOptions<ProfileContext> options = new DbContextOptionsBuilder<ProfileContext>()
            //     .UseInMemoryDatabase(databaseName: "client_profiles_test_db")
            //     .Options;
            //
            // Guid householdId;
            // Guid expectedMeetingId;
            //
            // // ========== SETUP =========
            // using (ProfileContext context = new ProfileContext(options))
            // {
            //     HouseholdRepository repo = new HouseholdRepository(context);
            //
            //     Client c1 = new Client("c_1", "c1@household_repo.test", DateTimeOffset.MinValue);
            //     Household h = new Household(c1);
            //
            //     Meeting meeting = h.BeginMeeting(new Advisor(Guid.NewGuid(), "TST", "TST", ""), 1803);
            //
            //     householdId = h.Guid;
            //     expectedMeetingId = meeting.Guid;
            //
            //     repo.AddHousehold(h);
            //     context.SaveChanges();
            // }
            //
            // // ========= ACT ==========
            // using (ProfileContext context = new ProfileContext(options))
            // {
            //     HouseholdRepository repo = new HouseholdRepository(context);
            //
            //     Household household = repo.FindById(householdId);
            //
            //     Assert.Contains(household.Meetings, p => p.Guid == expectedMeetingId);
            // }
        }
    }
}