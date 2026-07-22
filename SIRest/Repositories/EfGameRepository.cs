using GameCollectionApi.Data;
using GameCollectionApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameCollectionApi.Repositories;

public sealed class EfGameRepository(GameDbContext context) : IGameRepository
{
    public async Task<IReadOnlyCollection<Game>> GetAllAsync()
    {
        return await context.Games
            .AsNoTracking()
            .OrderBy(game => game.Id)
            .ToListAsync();
    }

    public async Task<Game?> GetByIdAsync(int id)
    {
        return await context.Games
            .AsNoTracking()
            .SingleOrDefaultAsync(game => game.Id == id);
    }

    public async Task<Game> CreateAsync(Game game)
    {
        context.Games.Add(game);
        await context.SaveChangesAsync();
        return game;
    }

    public async Task<bool> UpdateAsync(Game game)
    {
        if (!await context.Games.AnyAsync(existingGame => existingGame.Id == game.Id))
        {
            return false;
        }

        context.Games.Update(game);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var deletedRows = await context.Games
            .Where(game => game.Id == id)
            .ExecuteDeleteAsync();

        return deletedRows > 0;
    }
}
