using BookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Controllers;

[ApiController]
[Route("api/admin/users")]
public class AdminUsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminUsersController(AppDbContext db)
    {
        _db = db;
    }

    // DEV ONLY: promote a user to Admin by email
    // POST /api/admin/users/promote?email=someone@example.com
[HttpPost("promote")]
public async Task<IActionResult> PromoteToAdmin([FromQuery] string email, [FromServices] IHostEnvironment env)
{
    if (!env.IsDevelopment())
        return Forbid();

    var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user is null)
        return NotFound("User not found.");

    user.Role = "Admin";
    await _db.SaveChangesAsync();

    return Ok($"User '{email}' is now Admin.");
}

}
