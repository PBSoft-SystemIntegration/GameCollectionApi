using GameCollectionApi.Models;

namespace GameCollectionApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
}
