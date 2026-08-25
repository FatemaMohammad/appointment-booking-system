using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Api.Contracts;

public record RegisterRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);
