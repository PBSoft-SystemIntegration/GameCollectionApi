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

        [HttpGet("{id:int}", Name = nameof(GetById))]
        [EndpointSummary("Get a game by id")]
        [EndpointDescription("Returns one game when the id exists.")]
        [ProducesResponseType<GameResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            return Games.TryGetValue(id, out var game)
                ? Ok(ToResponse(id, game))
                : NotFound();
        }

        [HttpPost]
        [EndpointSummary("Create a game")]
        [EndpointDescription("Adds a game and returns the created resource with its location.")]
        [ProducesResponseType<GameResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        public IActionResult Create(CreateGameRequest request)
        {
            var id = Games.Count == 0 ? 0 : Games.Keys.Max() + 1;
            var game = new Game
            {
                Title = request.Title,
                Genre = request.Genre,
                ReleaseYear = request.ReleaseYear
            };

            Games[id] = game;
            var response = ToResponse(id, game);
            return CreatedAtRoute(nameof(GetById), new { id }, response);
        }

        [HttpPut("{id:int}")]
        [EndpointSummary("Replace a game")]
        [EndpointDescription("Replaces all editable fields on an existing game.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public IActionResult Replace(int id, UpdateGameRequest request)
        {
            if (!Games.ContainsKey(id))
            {
                return NotFound();
            }

            Games[id] = new Game
            {
                Title = request.Title,
                Genre = request.Genre,
                ReleaseYear = request.ReleaseYear
            };

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [EndpointSummary("Update part of a game")]
        [EndpointDescription("Changes only the supplied fields on an existing game.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public IActionResult UpdatePartially(int id, PatchGameRequest request)
        {
            if (!Games.TryGetValue(id, out var game))
            {
                return NotFound();
            }

            if (request.Title is null && request.Genre is null && request.ReleaseYear is null)
            {
                ModelState.AddModelError(nameof(request), "At least one field must be supplied.");
                return ValidationProblem(ModelState);
            }

            game.Title = request.Title ?? game.Title;
            game.Genre = request.Genre ?? game.Genre;
            game.ReleaseYear = request.ReleaseYear ?? game.ReleaseYear;
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [EndpointSummary("Delete a game")]
        [EndpointDescription("Removes a game from the collection.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            return Games.Remove(id) ? NoContent() : NotFound();
        }

        private static GameResponse ToResponse(int id, Game game) =>
            new(id, game.Title, game.Genre, game.ReleaseYear);
    }
}
