using Microsoft.EntityFrameworkCore;

namespace lab4.Models
{
    public class MuseumContext : DbContext
    {
        public DbSet<Exhibition> Exhibitions { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Tour> Tours { get; set; }

        public MuseumContext(DbContextOptions<MuseumContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
