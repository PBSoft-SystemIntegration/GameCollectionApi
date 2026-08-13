using GameCollectionApi.Data;
using GameCollectionApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameCollectionApi.Repositories;

public sealed class EfUserRepository(GameDbContext context) : IUserRepository
{
    public Task<User?> GetByUsernameAsync(string username)
    {
        return context.Users.SingleOrDefaultAsync(user => user.Username == username);
    }

    public Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash)
    {
        return context.Users.SingleOrDefaultAsync(user =>
            user.RefreshTokenHash == refreshTokenHash);
    }

    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }
}
