using BookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ServicesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var services = await _db.Services
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return Ok(services);
    }
}
