using BookingSystem.Api.Contracts;
using BookingSystem.Application.Auth;
using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Controllers;
[AllowAnonymous]
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly AppPasswordHasher _hasher;
    private readonly JwtTokenService _jwt;

    public AuthController(AppDbContext db, AppPasswordHasher hasher, JwtTokenService jwt)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email already exists");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = _hasher.Hash(request.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null || !_hasher.Verify(user.PasswordHash, request.Password))
            return Unauthorized();

        var token = _jwt.Generate(user);
        return Ok(new { token });
    }
}
