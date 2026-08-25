using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Api.Contracts;

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);
