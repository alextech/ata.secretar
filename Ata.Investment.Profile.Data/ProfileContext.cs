using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.Household;

namespace Ata.Investment.Profile.Data
{
    public class ProfileContext : DbContext
    {
        public ProfileContext(DbContextOptions<ProfileContext> options)
            : base(options)
        {
        }

        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Household> Households { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // =========== HOUSEHOLD CLIENT  =================
            var clientEntity = modelBuilder
                .HasDefaultSchema("Investments")
                .Entity<Client>();
            clientEntity.Property<int>("Id")
                .ValueGeneratedOnAdd();
            clientEntity.HasKey("Id");

            clientEntity
                .HasIndex(c => c.Guid)
                .IsUnique();

            clientEntity
                .HasIndex(e =>new { e.Name, e.Email })
                .IsUnique();

            // ========= HOUSEHOLD ===========
            var householdEntity = modelBuilder
                .HasDefaultSchema("Investments")
                .Entity<Household>();

            householdEntity.Property<int>("Id")
                .ValueGeneratedOnAdd();
            householdEntity.HasKey("Id");

            householdEntity.Property<int>("PrimaryClientId");
            householdEntity.Property<int?>("JointClientId");

            householdEntity
                .HasOne(h => h.PrimaryClient)
                .WithMany()
                .HasForeignKey("PrimaryClientId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            householdEntity
                .HasOne(h => h.JointClient)
                .WithMany()
                .HasForeignKey("JointClientId")
                .OnDelete(DeleteBehavior.Restrict);

            householdEntity.HasMany(h => h.Meetings);

            // ======= MEETING ==========
            var meetingEntity = modelBuilder
                .HasDefaultSchema("Investments")
                .Entity<Meeting>();

            meetingEntity.Property<int>("Id")
                .ValueGeneratedOnAdd();
            meetingEntity.HasKey("Id");

            meetingEntity.Ignore(m => m.KycDocument);
            meetingEntity.Property(m => m.XmlKycDocumentSource).HasColumnName("KycDocument");

            EventStorage(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        // TODO move to own class
        private void EventStorage(ModelBuilder modelBuilder)
        {
            
        }

        public override int SaveChanges()
        {
            SyncDocumentWithMeeting();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            SyncDocumentWithMeeting();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void SyncDocumentWithMeeting()
        {
            // foreach (EntityEntry entityEntry in ChangeTracker.Entries())
            // {
            //     if (entityEntry.Entity.GetType() != typeof(Meeting)) continue;
            //
            //     Meeting meeting = (Meeting) entityEntry.Entity;
            //
            //     // server side: meeting creation, meeting cloning. Server code does not generate xml.
            //     // if (meeting.XmlKycDocumentSource == null)
            //     // {
            //     //     entityEntry.Property(nameof(Meeting.XmlKycDocumentSource)).CurrentValue =
            //     //         KycDocumentEncoder.Encode(meeting.KycDocument);
            //     // }
            // }
        }
    }
}