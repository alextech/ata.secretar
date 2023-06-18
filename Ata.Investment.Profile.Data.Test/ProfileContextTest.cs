using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedTestingKernel;
using Xunit.Abstractions;

namespace Ata.Investment.Profile.Data.Test
{
    public class ProfileContextTest : IDisposable
    {
        public ProfileContextTest(ITestOutputHelper output)
        {
            _loggerFactory = DbLoggerFactory.CreateLoggerForOutput(output);
            _optionsBuilder.UseSqlServer(
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Ata.Investment.Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        public void Dispose()
        {
            _optionsBuilder.UseLoggerFactory(null);
            using (ProfileContext context = new ProfileContext(_optionsBuilder.Options))
            {
                context.Database.ExecuteSqlRaw("DELETE FROM [Investments].[Profiles]");
                context.Database.ExecuteSqlRaw("DELETE FROM [Investments].[FinancialState]");
                context.Database.ExecuteSqlRaw("DELETE FROM [Investments].[Clients]");
            }
        }

        private readonly ILoggerFactory _loggerFactory;

        private readonly DbContextOptionsBuilder<ProfileContext> _optionsBuilder
            = new DbContextOptionsBuilder<ProfileContext>();

//        [Fact (Skip = "Database not ready")]
//        public void SaveLoadProfileTest()
//        {
//            string guid;
//
//            using (ProfileContext context = new ProfileContext(_optionsBuilder.Options))
//            {
//                Client client = new Client("Profile Context", "client_1@profile.test", DateTime.Now);
//                client.AddFinancialState(
//                    new FinancialState(
//                        DateTime.Now,
//                        100000,
//                        new NetWorth(10, 10, 1),
//                        4
//                    )
//                );
//                Domain.Profile.Profile profile = new Domain.Profile.Profile(DateTime.Now, client)
//                {
//                    Name = "SaveLoadProfileTest",
//                    Objectives = new Objectives(50, 40, 10, 0, 0),
//                    RiskTolerance = new RiskTolerance(40, 60, 0, 0, 0),
//                    TimeHorizon = 10,
//                    Decline = 11,
//                    AnnualReturn = 12,
//                    CurrentInvestment = 13,
//                    Accounts = new[] {"RRSP", "TFSA"},
//                    InitialInvestment = 100,
//                    MonthlyCommitment = 49
//                };
//
//
//                guid = profile.Guid.ToString();
//
//                context.Add(client);
//                context.SaveChanges();
//                context.Add(profile);
//                context.SaveChanges();
//            }
//
//            _optionsBuilder.UseLoggerFactory(_loggerFactory);
//            using (ProfileContext context = new ProfileContext(_optionsBuilder.Options))
//            {
//                var profiles =
//                    from p in context.Profiles.AsNoTracking()
//                    where p.Guid.ToString() == guid
//                    select p;
//
//                Domain.Profile.Profile profile = profiles.First();
//
//                Assert.Equal("SaveLoadProfileTest", profile.Name);
//            }
//        }
    }
}