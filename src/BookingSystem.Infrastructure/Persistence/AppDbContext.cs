using Microsoft.EntityFrameworkCore;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;

namespace BookingSystem.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Service> Services => Set<Service>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BusinessHours> BusinessHours => Set<BusinessHours>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // -------------------------
        // Service
        // -------------------------
        modelBuilder.Entity<Service>().HasData(
            new Service { Id = 1, Name = "Consultation", DurationMinutes = 30, Price = 0, IsActive = true },
            new Service { Id = 2, Name = "Teeth Cleaning", DurationMinutes = 45, Price = 500, IsActive = true },
            new Service { Id = 3, Name = "Follow-up", DurationMinutes = 15, Price = 0, IsActive = true }
        );

        // -------------------------
        // TimeSlot configuration
        // -------------------------
        modelBuilder.Entity<TimeSlot>()
            .Property(t => t.Status)
            .HasConversion<string>();

        // Prevent duplicate slots for same service at same start time
        modelBuilder.Entity<TimeSlot>()
            .HasIndex(t => new { t.ServiceId, t.StartUtc })
            .IsUnique();

        // Seeded example TimeSlots
        modelBuilder.Entity<TimeSlot>().HasData(
            new TimeSlot
            {
                Id = 1,
                ServiceId = 1,
                StartUtc = new DateTime(2026, 02, 10, 08, 00, 00, DateTimeKind.Utc),
                EndUtc = new DateTime(2026, 02, 10, 08, 30, 00, DateTimeKind.Utc),
                Status = TimeSlotStatus.Available
            },
            new TimeSlot
            {
                Id = 2,
                ServiceId = 1,
                StartUtc = new DateTime(2026, 02, 10, 08, 30, 00, 00, DateTimeKind.Utc),
                EndUtc = new DateTime(2026, 02, 10, 09, 00, 00, DateTimeKind.Utc),
                Status = TimeSlotStatus.Blocked
            },
            new TimeSlot
            {
                Id = 3,
                ServiceId = 2,
                StartUtc = new DateTime(2026, 02, 10, 09, 00, 00, DateTimeKind.Utc),
                EndUtc = new DateTime(2026, 02, 10, 09, 45, 00, 00, DateTimeKind.Utc),
                Status = TimeSlotStatus.Available
            }
        );

        // -------------------------
        // Booking configuration
        // -------------------------
        modelBuilder.Entity<Booking>()
            .Property(b => b.Status)
            .HasConversion<string>();

        // IMPORTANT:
        // We do NOT keep a unique index on TimeSlotId anymore,
        // because cancelled bookings should not block the same slot forever.
        // Active-booking conflict should instead be checked in application code.

        // Optional relationships
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.TimeSlot)
            .WithMany()
            .HasForeignKey(b => b.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Service)
            .WithMany()
            .HasForeignKey(b => b.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        // -------------------------
        // BusinessHours
        // -------------------------
        modelBuilder.Entity<BusinessHours>().HasData(
            new BusinessHours { Id = 1, DayOfWeek = 1, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(16, 0, 0) }, // Monday
            new BusinessHours { Id = 2, DayOfWeek = 2, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(16, 0, 0) }, // Tuesday
            new BusinessHours { Id = 3, DayOfWeek = 3, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(16, 0, 0) }, // Wednesday
            new BusinessHours { Id = 4, DayOfWeek = 4, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(16, 0, 0) }, // Thursday
            new BusinessHours { Id = 5, DayOfWeek = 5, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(16, 0, 0) }  // Friday
        );
    }
}