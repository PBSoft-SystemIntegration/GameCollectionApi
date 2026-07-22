using GameCollectionApi.Models;

namespace GameCollectionApi.Repositories;

public sealed class InMemoryGameRepository : IGameRepository
{
    private readonly Dictionary<int, Game> _games = new()
    {
        [0] = new() { Id = 0, Title = "Tomb Raider", Genre = "Action", ReleaseYear = 1996 },
        [1] = new() { Id = 1, Title = "Elden Ring", Genre = "RPG", ReleaseYear = 2022 }
    };

    public Task<IReadOnlyCollection<Game>> GetAllAsync()
    {
        IReadOnlyCollection<Game> games = _games.Values.ToList();
        return Task.FromResult(games);
    }

    public Task<Game?> GetByIdAsync(int id)
    {
        return Task.FromResult(_games.GetValueOrDefault(id));
    }

    public Task<Game> CreateAsync(Game game)
    {
        game.Id = _games.Count == 0 ? 0 : _games.Keys.Max() + 1;
        _games[game.Id] = game;
        return Task.FromResult(game);
    }

    public Task<bool> UpdateAsync(Game game)
    {
        if (!_games.ContainsKey(game.Id))
        {
            return Task.FromResult(false);
        }

        _games[game.Id] = game;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return Task.FromResult(_games.Remove(id));
    }
}
