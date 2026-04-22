using BookingSystem.Application.Slots;
using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace BookingSystem.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/slots")]
public class AdminSlotsController : ControllerBase
{
   private readonly AppDbContext _db;
private readonly SlotGenerator _generator;

public AdminSlotsController(AppDbContext db, SlotGenerator generator)
{
    _db = db;
    _generator = generator;
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

    [HttpGet("whoami")]
public IActionResult WhoAmI()
{
    return Ok(new
    {
        isAuthenticated = User.Identity?.IsAuthenticated,
        roleClaims = User.Claims
            .Where(c => c.Type.Contains("role"))
            .Select(c => new { c.Type, c.Value })
            .ToList(),
        isAdmin = User.IsInRole("Admin")
    });
}

}
