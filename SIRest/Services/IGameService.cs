using GameCollectionApi.Models;

namespace GameCollectionApi.Services;

public interface IGameService
{
    Task<IReadOnlyCollection<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(int id);
    Task<Game> CreateAsync(Game game);
    Task<bool> ReplaceAsync(int id, Game game);
    Task<bool> UpdatePartiallyAsync(
        int id,
        string? title,
        string? genre,
        ushort? releaseYear);
    Task<bool> DeleteAsync(int id);
}
