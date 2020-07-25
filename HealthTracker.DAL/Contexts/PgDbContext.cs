using HealthTracker.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthTracker.DAL.Contexts
{
    public class PgDbContext : DbContext
    {
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Controller> Controllers { get; set; }
        public PgDbContext(DbContextOptions<PgDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Controller>().Property(n => n.IsOnline).HasDefaultValue(false);

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

    }
}
