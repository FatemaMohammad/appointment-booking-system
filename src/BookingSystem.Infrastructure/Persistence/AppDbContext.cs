using Microsoft.EntityFrameworkCore;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;


namespace BookingSystem.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    //DbSet<T> er den EF Core-mekanisme der repræsenterer en tabel.
    public DbSet<Service> Services => Set<Service>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BusinessHours> BusinessHours => Set<BusinessHours>();
    public DbSet<User> Users => Set<User>();

//Seed data - forudfyldning af databasen med nogle standard services
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Service>().HasData(
            new Service { Id = 1, Name = "Consultation", DurationMinutes = 30, Price = 0, IsActive = true },
            new Service { Id = 2, Name = "Teeth Cleaning", DurationMinutes = 45, Price = 500, IsActive = true },
            new Service { Id = 3, Name = "Follow-up", DurationMinutes = 15, Price = 0, IsActive = true }
        );
         // TimeSlot seed for 2026-02-10 in Copenhagen time.
        // Feb in Denmark is CET (UTC+1), so 09:00 local = 08:00Z.
        modelBuilder.Entity<TimeSlot>().HasData(
            new TimeSlot
            {
                Id = 1,
                ServiceId = 1,
                StartUtc = new DateTime(2026, 02, 10, 08, 00, 00, DateTimeKind.Utc),
                EndUtc   = new DateTime(2026, 02, 10, 08, 30, 00, DateTimeKind.Utc),
                Status = TimeSlotStatus.Available
            },
            new TimeSlot
            {
                Id = 2,
                ServiceId = 1,
                StartUtc = new DateTime(2026, 02, 10, 08, 30, 00, DateTimeKind.Utc),
                EndUtc   = new DateTime(2026, 02, 10, 09, 00, 00, DateTimeKind.Utc),
                Status = TimeSlotStatus.Blocked
            },
            new TimeSlot
            {
                Id = 3,
                ServiceId = 2,
                StartUtc = new DateTime(2026, 02, 10, 09, 00, 00, DateTimeKind.Utc),
                EndUtc   = new DateTime(2026, 02, 10, 09, 45, 00, DateTimeKind.Utc),
                Status = TimeSlotStatus.Available
            }
        );
        modelBuilder.Entity<Booking>()
    .Property(b => b.Status)
    .HasConversion<string>();

// 1 booking per TimeSlot (interview-vigtig del)
modelBuilder.Entity<Booking>()
    .HasIndex(b => b.TimeSlotId)
    .IsUnique();

    modelBuilder.Entity<BusinessHours>().HasData(
    new BusinessHours { Id = 1, DayOfWeek = 1, OpenTime = new TimeSpan(9,0,0), CloseTime = new TimeSpan(16,0,0) }, // Monday
    new BusinessHours { Id = 2, DayOfWeek = 2, OpenTime = new TimeSpan(9,0,0), CloseTime = new TimeSpan(16,0,0) }, // Tuesday
    new BusinessHours { Id = 3, DayOfWeek = 3, OpenTime = new TimeSpan(9,0,0), CloseTime = new TimeSpan(16,0,0) }, // Wednesday
    new BusinessHours { Id = 4, DayOfWeek = 4, OpenTime = new TimeSpan(9,0,0), CloseTime = new TimeSpan(16,0,0) }, // Thursday
    new BusinessHours { Id = 5, DayOfWeek = 5, OpenTime = new TimeSpan(9,0,0), CloseTime = new TimeSpan(16,0,0) }  // Friday
);


    }
}

