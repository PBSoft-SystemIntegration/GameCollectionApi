using GameCollectionApi.Models;

namespace GameCollectionApi.Services
{
    public interface IGameService
    {
        IReadOnlyCollection<Game> GetAll();
        Game? GetById(int id);
        Game Create(Game game);
        bool Replace(int id, Game game);
        bool UpdatePartially(int id, string? title, string? genre, ushort? releaseYear);
        bool Delete(int id);
    }
}
