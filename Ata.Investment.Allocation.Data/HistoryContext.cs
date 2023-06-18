using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Allocation.Domain;

namespace Ata.Investment.Allocation.Data
{
    public class HistoryContext : DbContext
    {
        public DbContextOptions<HistoryContext> Options { get; private set; }
        public DbSet<FundSyncDTO> FundHistorySyncQuery { get; set; }
        
        public HistoryContext(DbContextOptions<HistoryContext> options)
            : base(options)
        {
            Options = options;
        }
        
        public async Task RecordHistory(string fundCode, List<HistoryDay> historyDays)
        {

            StringBuilder insertBuilder = new StringBuilder("INSERT INTO [FundsHistory].[DailyHistory] (FundCode, Day, Value) VALUES ");
            int sqlInitLength = insertBuilder.Length;

            int from = 0, to = (historyDays.Count > 100) ? 100 : historyDays.Count;

            bool isAtEnd = false;
            for (; from < to; from += 100)
            {
                insertBuilder.Length = sqlInitLength;

                List<SqlParameter> parameters = new List<SqlParameter>();
                for (int i = 0, j = 0; j < (to-from); i += 3, j++)
                {
                    if (from + j >= historyDays.Count)
                    {

                        isAtEnd = true;
                        break;
                    }

                    insertBuilder.Append($"(@{i}, @{i + 1}, @{i + 2}),");



                    parameters.Add(new SqlParameter("@"+i.ToString(), fundCode));
                    parameters.Add(new SqlParameter("@"+(i + 1).ToString(), historyDays[from+j].Day));
                    parameters.Add(new SqlParameter("@"+(i + 2).ToString(), historyDays[from+j].Value));
                }

                insertBuilder.Remove(insertBuilder.Length - 1, 1);

                string insertSql = insertBuilder.ToString();

                await Database.ExecuteSqlRawAsync(insertSql, parameters);
                
                if (isAtEnd)
                {
                    break;
                }
                
                to = (historyDays.Count > 100) ? to + 100 : historyDays.Count;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var historyDay = modelBuilder
                .HasDefaultSchema("FundsHistory")
                .Entity<HistoryDay>()
                .ToTable("DailyHistory");

            historyDay.HasKey(h => new {day = h.Day, h.FundCode});

            historyDay.Property(h => h.Day)
                .HasField("_i");

            historyDay.Property(h => h.Value)
                .HasField("_v")
                .HasColumnType("decimal(8,5)");

            var fundSyncView = modelBuilder.Entity<FundSyncDTO>()
                .ToTable("LastSync");
            fundSyncView.Property(f => f.FundCode).HasColumnName("FundCode");
            fundSyncView.Property(f => f.MsUrl).HasColumnName("MsUrl");
            fundSyncView.Property(f => f.MsCode).HasColumnName("MsCode");
            fundSyncView.Property(f => f.LastSync).HasColumnName("LastSync");

            fundSyncView.HasNoKey();

            base.OnModelCreating(modelBuilder);
        }
    }
}