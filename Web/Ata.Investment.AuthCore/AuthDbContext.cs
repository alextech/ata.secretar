using History.Cmd;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ata.Investment.AuthCore
{
    public class AuthDbContext : IdentityDbContext<Advisor>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public DbSet<AccessLog> AccessLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            EntityTypeBuilder<AccessLog> logEntity = modelBuilder
                .Entity<AccessLog>();
            logEntity.HasKey(l => l.TimeStamp);
        }
    }
}