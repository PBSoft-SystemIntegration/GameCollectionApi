namespace GameCollectionApi.Services;

public sealed record AuthTokens(
    string Token,
    string RefreshToken);
