using BookingSystem.Domain.Enums;
namespace BookingSystem.Domain.Entities;

public class TimeSlot
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public TimeSlotStatus Status { get; set; } = TimeSlotStatus.Available;
    public Service? Service { get; set; }
}