namespace BookingSystem.Api.Contracts;

public record CreateBookingRequest(int ServiceId, int TimeSlotId);
