using System.ComponentModel.DataAnnotations;

namespace GameCollectionApi.DTO;

public sealed record LoginRequest(
    [Required] string Username,
    [Required] string Password);
