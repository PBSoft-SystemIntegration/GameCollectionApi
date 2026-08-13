namespace GameCollectionApi.DTO;

public sealed record AuthResponse(
    string Token,
    string RefreshToken);
