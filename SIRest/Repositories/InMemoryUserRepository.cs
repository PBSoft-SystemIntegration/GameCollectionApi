using GameCollectionApi.Models;

namespace GameCollectionApi.Repositories;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users =
    [
        new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = "Admin"
        }
    ];

    public Task<User?> GetByUsernameAsync(string username)
    {
        var user = _users.FirstOrDefault(user =>
            user.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash)
    {
        var user = _users.FirstOrDefault(user =>
            user.RefreshTokenHash == refreshTokenHash);
        return Task.FromResult(user);
    }

    public Task<User> CreateAsync(User user)
    {
        user.Id = _users.Count == 0 ? 1 : _users.Max(existing => existing.Id) + 1;
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        return Task.CompletedTask;
    }
}
