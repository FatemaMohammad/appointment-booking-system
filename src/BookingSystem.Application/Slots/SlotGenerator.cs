using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;

namespace BookingSystem.Application.Slots;

public class SlotGenerator
{
    public List<TimeSlot> GenerateSlots(
        Service service,
        BusinessHours hours,
        DateOnly date,
        TimeZoneInfo tz)
    {
        var slots = new List<TimeSlot>();

        var localStart = date.ToDateTime(TimeOnly.FromTimeSpan(hours.OpenTime));
        var localEnd = date.ToDateTime(TimeOnly.FromTimeSpan(hours.CloseTime));

        var duration = TimeSpan.FromMinutes(service.DurationMinutes);

        var current = localStart;

        while (current + duration <= localEnd)
        {
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(current, tz);
            var endUtc = TimeZoneInfo.ConvertTimeToUtc(current + duration, tz);

            slots.Add(new TimeSlot
            {
                ServiceId = service.Id,
                StartUtc = startUtc,
                EndUtc = endUtc,
                Status = TimeSlotStatus.Available
            });

            current += duration;
        }

        return slots;
    }
}
