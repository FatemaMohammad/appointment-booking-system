using BookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BookingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvailabilityController : ControllerBase
{
    private readonly AppDbContext _db;

    public AvailabilityController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/availability?serviceId=1&date=2026-02-10
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int serviceId, [FromQuery] string date)
    {
        if (serviceId <= 0)
            return BadRequest("serviceId must be a positive integer.");

        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var day))
            return BadRequest("date must be in format yyyy-MM-dd (e.g., 2026-02-10).");

        // Interpret the provided date as Europe/Copenhagen and query the corresponding UTC range
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");
        var localStart = day.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
        var localEnd = localStart.AddDays(1);

        var startUtc = TimeZoneInfo.ConvertTimeToUtc(localStart, tz);
        var endUtc = TimeZoneInfo.ConvertTimeToUtc(localEnd, tz);

        // Optional: ensure service exists (nice API behavior)
        var serviceExists = await _db.Services.AnyAsync(s => s.Id == serviceId && s.IsActive);
        if (!serviceExists)
            return NotFound($"Service {serviceId} not found (or inactive).");

        var slots = await _db.TimeSlots
            .Where(s => s.ServiceId == serviceId && s.StartUtc >= startUtc && s.StartUtc < endUtc)
            .OrderBy(s => s.StartUtc)
            .Select(s => new
            {
                s.Id,
                s.StartUtc,
                s.EndUtc,
                Status = s.Status.ToString()
            })
            .ToListAsync();

        return Ok(slots);
    }
}
