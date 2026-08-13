namespace GameCollectionApi.Services;

public interface IAuthService
{
    Task<AuthTokens?> RegisterAsync(string username, string password);
    Task<AuthTokens?> LoginAsync(string username, string password);
    Task<AuthTokens?> RefreshAsync(string refreshToken);
    Task SeedAdminAsync();
}
