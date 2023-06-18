using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedTestingKernel;
using Ata.Investment.Allocation.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Ata.Investment.Allocation.Data.Test
{
    public class HistoryContextTest : IDisposable
    {
        public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(
            builder =>
            {
//                builder.AddFilter()
                builder.AddConsole();
            });

        public HistoryContextTest(ITestOutputHelper output)
        {
            _loggerFactory = DbLoggerFactory.CreateLoggerForOutput(output);
            _optionsBuilder.UseSqlServer(
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Ata.Investment.Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            _mapper = new MapperConfiguration(
                cfg => { cfg.CreateMap<IDataRecord, HistoryDay>(); }).CreateMapper();
        }

        [Fact]
        public async Task HistoryInsertTest()
        {
            List<HistoryDay> rawHistory = new List<HistoryDay>
            {
                new HistoryDay {FundCode = "cig4242", i = new DateTime(2019, 02, 12), v = new decimal(7.2)},
                new HistoryDay {FundCode = "pmo500", i = new DateTime(2019, 02, 13), v = new decimal(8.0)}
            };

            // setup
            using (HistoryContext context = new HistoryContext(_optionsBuilder.Options))
            {
                // act
                await context.AddRangeAsync(rawHistory);
                await context.SaveChangesAsync();
            }

            // test
            using (HistoryContext context = new HistoryContext(_optionsBuilder.Options))
            {
                var connection = context.Database.GetDbConnection();
                await connection.OpenAsync();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Day, FundCode, Value FROM [FundsHistory].[DailyHistory]";
                    DbDataReader reader = await command.ExecuteReaderAsync();

                    List<HistoryDay> days = new List<HistoryDay>();
                    while (await reader.ReadAsync())
                    {
                        days.Add(new HistoryDay
                            {FundCode = reader.GetString(1), i = reader.GetDateTime(0), v = reader.GetDecimal(2)});
                    }

                    Assert.Equal(2, days.Count);
                    Assert.Contains(rawHistory[0], days);
                    Assert.Contains(rawHistory[1], days);
                }
            }
        }

        [Fact]
        public async Task RecordHistoryWithBatchTest()
        {
            // setup
            List<HistoryDay> rawHistory = new List<HistoryDay>();
            DateTime dt = new DateTime(2019, 01, 01);

            for (int i = 0; i < 110; i++, dt = dt.AddDays(1))
            {
                rawHistory.Add(new HistoryDay {FundCode = "cig4242", i = dt, v = new decimal(7.2)});
            }

            using (HistoryContext context = new HistoryContext(_optionsBuilder.Options))
            {
                // act
                await context.RecordHistory("cig4242", rawHistory);
                await context.SaveChangesAsync();
            }

            // test assert
            // test
            using (HistoryContext context = new HistoryContext(_optionsBuilder.Options))
            {
                var connection = context.Database.GetDbConnection();
                await connection.OpenAsync();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(1) FROM [FundsHistory].[DailyHistory]";
                    DbDataReader reader = await command.ExecuteReaderAsync();

                    int numRecords = 0;
                    while (await reader.ReadAsync())
                    {
                        numRecords = reader.GetInt32(0);
                    }

                    Assert.Equal(110, numRecords);
                }
            }
        }

        private readonly ILoggerFactory _loggerFactory;

        private readonly DbContextOptionsBuilder<HistoryContext> _optionsBuilder
            = new DbContextOptionsBuilder<HistoryContext>();

        private IMapper _mapper;

        public void Dispose()
        {
            using (HistoryContext context = new HistoryContext(_optionsBuilder.Options))
            {
                context.Database.ExecuteSqlRaw("DELETE FROM [FundsHistory].[DailyHistory]");
            }
        }
    }
}