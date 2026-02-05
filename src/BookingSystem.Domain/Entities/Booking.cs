using BookingSystem.Domain.Entities;
 
 namespace BookingSystem.Domain.Enums;
public class Booking
{
    public int Id { get; set; }

    public int UserId { get; set; }          // senere: fra JWT
    public int ServiceId { get; set; }
    public int TimeSlotId { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Active;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CancelledAtUtc { get; set; }

    // Optional navigation
    public Service? Service { get; set; }
    public TimeSlot? TimeSlot { get; set; }
}   