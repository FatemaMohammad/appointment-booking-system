using BookingSystem.Application.Slots;
using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Controllers;

[ApiController]
[Route("api/admin/slots")]
public class AdminSlotsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly SlotGenerator _generator = new();

    public AdminSlotsController(AppDbContext db)
    {
        _db = db;
    }

    // POST /api/admin/slots/generate?serviceId=1&from=2026-02-10&to=2026-02-14
    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
        int serviceId,
        DateOnly from,
        DateOnly to)
    {
        var service = await _db.Services.FirstOrDefaultAsync(s => s.Id == serviceId && s.IsActive);
        if (service is null)
            return NotFound("Service not found");

        var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");

        var businessHours = await _db.BusinessHours.ToListAsync();
        var newSlots = new List<TimeSlot>();

        for (var date = from; date <= to; date = date.AddDays(1))
        {
            var bh = businessHours.FirstOrDefault(b => b.DayOfWeek == (int)date.DayOfWeek);
            if (bh == null) continue;

            var generated = _generator.GenerateSlots(service, bh, date, tz);

            foreach (var slot in generated)
            {
                var exists = await _db.TimeSlots.AnyAsync(s =>
                    s.ServiceId == slot.ServiceId &&
                    s.StartUtc == slot.StartUtc);

                if (!exists)
                    newSlots.Add(slot);
            }
        }

        _db.TimeSlots.AddRange(newSlots);
        await _db.SaveChangesAsync();

        return Ok(new { created = newSlots.Count });
    }
}
