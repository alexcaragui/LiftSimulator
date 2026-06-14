using Lift.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Lift.Data
{
    public class LiftDbContext : DbContext
    {
        public LiftDbContext(DbContextOptions<LiftDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<LiftEvent> LiftEvents { get; set; }
        public DbSet<ProcessInterruption> ProcessInterruptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurare User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Role).IsRequired().HasMaxLength(20);
                entity.HasIndex(u => u.Username).IsUnique();
            });

            // Configurare LiftEvent
            modelBuilder.Entity<LiftEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
                entity.Property(e => e.EventType).IsRequired().HasMaxLength(50);
            });

            // Configurare ProcessInterruption
            modelBuilder.Entity<ProcessInterruption>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.HasOne(p => p.StoppedByUser)
                      .WithMany()
                      .HasForeignKey(p => p.StoppedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.RestartedByUser)
                      .WithMany()
                      .HasForeignKey(p => p.RestartedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed - un admin default
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                CreatedAt = new DateTime(2024, 1, 1)
            });
        }
    }
}