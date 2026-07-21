using Microsoft.AspNetCore.Mvc;
using GameCollectionApi.DTO;
using GameCollectionApi.Models;

namespace GameCollectionApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private static readonly Dictionary<int, Game> Games = new()
        {
            [0] = new() { Title = "Tomb Raider", Genre = "Action", ReleaseYear = 1996 },
            [1] = new() { Title = "Elden Ring", Genre = "RPG", ReleaseYear = 2022 }
        };

        [HttpGet]
        [EndpointSummary("Get all games")]
        [EndpointDescription("Returns every game in the collection.")]
        [ProducesResponseType<IReadOnlyCollection<GameResponse>>(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var response = Games
              .Select(entry => new GameResponse(
                entry.Key,
                entry.Value.Title,
                entry.Value.Genre,
                entry.Value.ReleaseYear))
              .ToList();
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [EndpointSummary("Get a game by id")]
        [EndpointDescription("Returns one game when the id exists.")]
        [ProducesResponseType<GameResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            if (!Games.TryGetValue(id, out var game))
            {
                return NotFound();
            }

            var response = new GameResponse(
              id,
              game.Title,
              game.Genre,
              game.ReleaseYear);

            return Ok(response);
        }

    }
}
