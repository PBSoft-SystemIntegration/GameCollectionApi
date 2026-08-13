using System.ComponentModel.DataAnnotations;

namespace GameCollectionApi.DTO;

public sealed record RefreshTokenRequest(
    [Required] string RefreshToken);
