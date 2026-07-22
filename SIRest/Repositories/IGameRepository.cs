using GameCollectionApi.Models;

namespace GameCollectionApi.Repositories;

public interface IGameRepository
{
    Task<IReadOnlyCollection<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(int id);
    Task<Game> CreateAsync(Game game);
    Task<bool> UpdateAsync(Game game);
    Task<bool> DeleteAsync(int id);
}
