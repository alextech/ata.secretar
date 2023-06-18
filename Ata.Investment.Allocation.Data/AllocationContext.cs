using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Ata.Investment.Allocation.Domain;
using Ata.Investment.Allocation.Domain.Composition;

namespace Ata.Investment.Allocation.Data
{
    public class AllocationContext : DbContext
    {
        public AllocationContext(DbContextOptions<AllocationContext> options)
            : base(options)
        {
        }

        public DbSet<Domain.Allocation> Allocations { get; set; }

        public DbSet<AllocationVersion> AllocationVersions { get; set; }
        
        public DbSet<VersionDraft> VersionDrafts { get; set; }
        
        public DbSet<Fund> Funds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var allocationEntity = modelBuilder
                .HasDefaultSchema("FundsV1")
                .Entity<Domain.Allocation>();

            allocationEntity.HasKey(e => e.RiskLevel);
            allocationEntity
                .HasMany(typeof(AllocationVersion), "_history")
                .WithOne();

            var allocationVersionEntity = modelBuilder
                .HasDefaultSchema("FundsV1")
                .Entity<AllocationVersion>();
            allocationVersionEntity.Property("Version").ValueGeneratedNever();

            allocationVersionEntity
                .HasKey(av => new {av.RiskLevel, av.Version});
            IMutableNavigation navigation = allocationVersionEntity
                .Metadata.FindNavigation(nameof(AllocationVersion.Options));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            var option = modelBuilder
                .HasDefaultSchema("FundsV1")
                .Entity<Option>();

            option.Property<int>("Id")
                .ValueGeneratedOnAdd();
            option.HasKey("Id");

            var allocationOption = modelBuilder
                .HasDefaultSchema("FundsV1")
                .Entity<AllocationOption>()
                .ToTable("AllocationOptions");

            // its PK key could possibly be just version+allocation_version_id
            allocationOption.OwnsMany(
                o => o.CompositionParts, cp =>
                {
                    cp.WithOwner()
                        .HasForeignKey("OptionId");
                    cp.HasOne(cpp => cpp.Fund)
                        .WithMany()
                        .HasForeignKey(cpp => cpp.FundCode);
                });
            allocationOption
                .HasOne(ao => ao.Option);

            var fundEntity = modelBuilder
                .HasDefaultSchema("FundsV1")
                .Entity<Fund>();
            fundEntity.HasKey(f => f.FundCode);
            
            var draftEntity = modelBuilder
                .HasDefaultSchema("FundsV1")
                .Entity<VersionDraft>();

            draftEntity.Property<int>("Id")
                .ValueGeneratedOnAdd();
            
            draftEntity.HasIndex(d => d.Version);

            draftEntity.Property(d => d.Draft);

            #region Seed

            modelBuilder
                .Entity<Domain.Allocation>()
                .HasData(
                    new {Id = 1, Guid=Guid.NewGuid(), Name = "Safety", RiskLevel = 1},
                    new {Id = 2, Guid=Guid.NewGuid(), Name = "Conservative Income", RiskLevel = 2},
                    new {Id = 3, Guid=Guid.NewGuid(), Name = "Balanced", RiskLevel = 3},
                    new {Id = 4, Guid=Guid.NewGuid(), Name = "Growth", RiskLevel = 4},
                    new {Id = 5, Guid=Guid.NewGuid(), Name = "Aggressive Growth", RiskLevel = 5}
                );

            #endregion

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var addedCompositionParts = (
                from cp in ChangeTracker.Entries()
                where cp.Entity is CompositionPart &&
                      cp.State == EntityState.Added
                select cp.Entity
            ).ToList();

            if (addedCompositionParts.Count > 0)
            {
                await InsertNewFundCodes(cancellationToken, addedCompositionParts);
            }
            
            
            
            return await base.SaveChangesAsync(cancellationToken);
        }

        private async Task InsertNewFundCodes(CancellationToken cancellationToken, List<object> addedCompositionParts)
        {
            using (DbCommand tmpTableCreate = Database.GetDbConnection().CreateCommand())
            {
                tmpTableCreate.CommandText = "CREATE TABLE #tmp_funds([FundCode] VARCHAR(30))";

                await Database.GetDbConnection().OpenAsync(cancellationToken);
                await tmpTableCreate.ExecuteNonQueryAsync(cancellationToken);
            }

            using (DbCommand insertSubmittedFunds = Database.GetDbConnection().CreateCommand())
            {
                StringBuilder sb = new StringBuilder("INSERT INTO #tmp_funds VALUES");

                for (int i = 0; i < addedCompositionParts.Count; i++)
                {
                    CompositionPart cp = addedCompositionParts[i] as CompositionPart;
                    sb.Append("(@Fund_" + i + "),");
                    Debug.Assert(cp != null, nameof(cp) + " != null");
                    insertSubmittedFunds.Parameters.Add(new SqlParameter("@Fund_" + i, cp.FundCode));
                }

                sb.Remove(sb.Length - 1, 1);

                insertSubmittedFunds.CommandText = sb.ToString();
                await insertSubmittedFunds.ExecuteNonQueryAsync(cancellationToken);
            }

            using (DbCommand mergeFunds = Database.GetDbConnection().CreateCommand())
            {
                mergeFunds.CommandText =@"
                    INSERT INTO [FundsV1].[Funds] ([FundCode]) 
                    SELECT tf.[FundCode]
                    FROM #tmp_funds tf
                    EXCEPT
                    SELECT ff.[FundCode]
                    FROM [FundsV1].[Funds] ff";
                await mergeFunds.ExecuteNonQueryAsync(cancellationToken);

                Database.GetDbConnection().Close();
            }
        }
    }
}