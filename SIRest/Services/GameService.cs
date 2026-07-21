using GameCollectionApi.Models;

namespace GameCollectionApi.Services
{
    public class GameService : IGameService
    {
        private static readonly Dictionary<int, Game> Games = new()
        {
            [0] = new() { Id = 0, Title = "Tomb Raider", Genre = "Action", ReleaseYear = 1996 },
            [1] = new() { Id = 1, Title = "Elden Ring", Genre = "RPG", ReleaseYear = 2022 }
        };

        public IReadOnlyCollection<Game> GetAll()
        {
            return Games.Values.ToList();
        }

        public Game? GetById(int id)
        {
            return Games.GetValueOrDefault(id);
        }

        public Game Create(Game game)
        {
            var id = Games.Count == 0 ? 0 : Games.Keys.Max() + 1;
            game.Id = id;
            Games[id] = game;
            return game;
        }

        public bool Replace(int id, Game game)
        {
            if (!Games.ContainsKey(id))
            {
                return false;
            }

            game.Id = id;
            Games[id] = game;
            return true;
        }

        public bool UpdatePartially(
            int id,
            string? title,
            string? genre,
            ushort? releaseYear)
        {
            if (!Games.TryGetValue(id, out var game))
            {
                return false;
            }

            game.Title = title ?? game.Title;
            game.Genre = genre ?? game.Genre;
            game.ReleaseYear = releaseYear ?? game.ReleaseYear;
            return true;
        }

        public bool Delete(int id)
        {
            return Games.Remove(id);
        }
    }
}

