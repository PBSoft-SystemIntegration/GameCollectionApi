using System.ComponentModel.DataAnnotations;

namespace GameCollectionApi.DTO;

public sealed record RegisterUserRequest(
    [Required, StringLength(100, MinimumLength = 3)] string Username,
    [Required, StringLength(100, MinimumLength = 8)] string Password);
