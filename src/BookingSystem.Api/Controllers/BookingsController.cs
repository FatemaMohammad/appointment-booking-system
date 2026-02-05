using BookingSystem.Api.Contracts;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _db;

    public BookingsController(AppDbContext db)
    {
        _db = db;
    }

    // POST /api/bookings
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest req)
    {
        // Mock user (senere fra JWT)
        var userId = 1;

        // Validate service exists
        var service = await _db.Services.FirstOrDefaultAsync(s => s.Id == req.ServiceId && s.IsActive);
        if (service is null)
            return NotFound($"Service {req.ServiceId} not found (or inactive).");

        // Validate slot exists and matches service
        var slot = await _db.TimeSlots.FirstOrDefaultAsync(t =>
            t.Id == req.TimeSlotId &&
            t.ServiceId == req.ServiceId);

        if (slot is null)
            return NotFound("TimeSlot not found for that service.");

        // Business rule: cannot book in the past
        if (slot.StartUtc <= DateTime.UtcNow)
            return BadRequest("Cannot book a slot in the past.");

        // Slot must be available (not blocked)
        if (slot.Status != TimeSlotStatus.Available)
            return BadRequest("This slot is not available.");

        var booking = new Booking
        {
            UserId = userId,
            ServiceId = req.ServiceId,
            TimeSlotId = req.TimeSlotId,
            Status = BookingStatus.Active,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Bookings.Add(booking);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Unique constraint violation => someone already booked it
            return Conflict("This slot has already been booked.");
        }

        return CreatedAtAction(nameof(GetMyBookings), new { }, booking);
    }

    // GET /api/bookings/me
    [HttpGet("me")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = 1;

        var bookings = await _db.Bookings
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAtUtc)
            .Select(b => new
            {
                b.Id,
                b.ServiceId,
                b.TimeSlotId,
                Status = b.Status.ToString(),
                b.CreatedAtUtc,
                b.CancelledAtUtc
            })
            .ToListAsync();

        return Ok(bookings);
    }

    // POST /api/bookings/{id}/cancel
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = 1;

        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
        if (booking is null)
            return NotFound();

        if (booking.Status == BookingStatus.Cancelled)
            return BadRequest("Booking already cancelled.");

        var slot = await _db.TimeSlots.FirstAsync(t => t.Id == booking.TimeSlotId);

        // Business rule: cancel >= 24 hours before
        var minNoticeHours = 24;
        if (slot.StartUtc <= DateTime.UtcNow.AddHours(minNoticeHours))
            return BadRequest($"Cancellation must happen at least {minNoticeHours} hours before.");

        booking.Status = BookingStatus.Cancelled;
        booking.CancelledAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok();
    }
}
