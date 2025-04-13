using Microsoft.EntityFrameworkCore;
using Museum.DAL.Entities;

namespace Museum.DAL.Context
{
    public class MuseumContext : DbContext
    {
        public MuseumContext(DbContextOptions<MuseumContext> options) : base(options) { }

        public DbSet<Exhibition> Exhibitions { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Tour> Tours { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Exhibition>(entity =>
            {
                entity.HasKey(e => e.ExhibitionId);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Theme).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TargetAudience).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.HasKey(s => s.ScheduleId);
                entity.HasOne(s => s.Exhibition)
                    .WithMany(e => e.Schedules)
                    .HasForeignKey(s => s.ExhibitionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Visit>(entity =>
            {
                entity.HasKey(v => v.VisitId);
                entity.Property(v => v.VisitorName).IsRequired().HasMaxLength(100);
                entity.HasOne(v => v.Exhibition)
                    .WithMany(e => e.Visits)
                    .HasForeignKey(v => v.ExhibitionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Tour>(entity =>
            {
                entity.HasKey(t => t.TourId);
                entity.Property(t => t.GuideName).IsRequired().HasMaxLength(100);
                entity.HasOne(t => t.Exhibition)
                    .WithMany(e => e.Tours)
                    .HasForeignKey(t => t.ExhibitionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}