using Microsoft.EntityFrameworkCore;

namespace sh.vcp.ldap.ChangeTracking
{
    public sealed class ChangeTrackingDbContext : DbContext
    {
        public DbSet<Change> Changes { get; set; }

        public ChangeTrackingDbContext(DbContextOptions<ChangeTrackingDbContext> options) : base(options) {
        }
    }
}
