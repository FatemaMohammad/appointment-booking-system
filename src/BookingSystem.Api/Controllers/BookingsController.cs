using BookingSystem.Api.Contracts;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingSystem.Api.Controllers;

[Authorize]
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
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var service = await _db.Services
            .FirstOrDefaultAsync(s => s.Id == req.ServiceId && s.IsActive);

        if (service is null)
            return NotFound($"Service {req.ServiceId} not found or inactive.");

        var slot = await _db.TimeSlots
            .FirstOrDefaultAsync(t => t.Id == req.TimeSlotId && t.ServiceId == req.ServiceId);

        if (slot is null)
            return NotFound("TimeSlot not found for that service.");

        if (slot.StartUtc <= DateTime.UtcNow)
            return BadRequest("Cannot book a slot in the past.");

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
            return Conflict("This slot has already been booked.");
        }

        return CreatedAtAction(nameof(GetMyBookings), new { }, new
        {
            booking.Id,
            booking.UserId,
            booking.ServiceId,
            booking.TimeSlotId,
            Status = booking.Status.ToString(),
            booking.CreatedAtUtc
        });
    }

    // GET /api/bookings/me
    [HttpGet("me")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var bookings = await _db.Bookings
            .Where(b => b.UserId == userId)
            .Join(
                _db.TimeSlots,
                booking => booking.TimeSlotId,
                slot => slot.Id,
                (booking, slot) => new
                {
                    booking.Id,
                    booking.ServiceId,
                    booking.TimeSlotId,
                    StartUtc = slot.StartUtc,
                    EndUtc = slot.EndUtc,
                    Status = booking.Status.ToString(),
                    booking.CreatedAtUtc,
                    booking.CancelledAtUtc
                })
            .OrderByDescending(b => b.CreatedAtUtc)
            .ToListAsync();

        return Ok(bookings);
    }

    // POST /api/bookings/{id}/cancel
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var booking = await _db.Bookings
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (booking is null)
            return NotFound("Booking not found.");

        if (booking.Status == BookingStatus.Cancelled)
            return BadRequest("Booking is already cancelled.");

        var slot = await _db.TimeSlots
            .FirstOrDefaultAsync(t => t.Id == booking.TimeSlotId);

        if (slot is null)
            return NotFound("Associated time slot not found.");

        var minNoticeHours = 24;
        if (slot.StartUtc <= DateTime.UtcNow.AddHours(minNoticeHours))
            return BadRequest($"Cancellation must happen at least {minNoticeHours} hours before the appointment.");

        booking.Status = BookingStatus.Cancelled;
        booking.CancelledAtUtc = DateTime.UtcNow;

        // Make slot available again after cancellation
        slot.Status = TimeSlotStatus.Available;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Booking cancelled successfully.",
            bookingId = booking.Id,
            slotId = slot.Id,
            slotStatus = slot.Status.ToString()
        });
    }
}