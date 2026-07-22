using GameCollectionApi.Models;
using GameCollectionApi.Repositories;

namespace GameCollectionApi.Services;

public sealed class GameService(IGameRepository gameRepository) : IGameService
{
    public Task<IReadOnlyCollection<Game>> GetAllAsync()
    {
        return gameRepository.GetAllAsync();
    }

    public Task<Game?> GetByIdAsync(int id)
    {
        return gameRepository.GetByIdAsync(id);
    }

    public Task<Game> CreateAsync(Game game)
    {
        return gameRepository.CreateAsync(game);
    }

    public Task<bool> ReplaceAsync(int id, Game game)
    {
        game.Id = id;
        return gameRepository.UpdateAsync(game);
    }

    public async Task<bool> UpdatePartiallyAsync(
        int id,
        string? title,
        string? genre,
        ushort? releaseYear)
    {
        var game = await gameRepository.GetByIdAsync(id);
        if (game is null)
        {
            return false;
        }

        game.Title = title ?? game.Title;
        game.Genre = genre ?? game.Genre;
        game.ReleaseYear = releaseYear ?? game.ReleaseYear;
        return await gameRepository.UpdateAsync(game);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return gameRepository.DeleteAsync(id);
    }
}
