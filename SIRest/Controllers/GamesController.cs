using GameCollectionApi.DTO;
using GameCollectionApi.Models;
using GameCollectionApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameCollectionApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {

        private readonly IGameService _gameService;
        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        [EndpointSummary("Get all games")]
        [EndpointDescription("Returns every game in the collection.")]
        [ProducesResponseType<IReadOnlyCollection<GameResponse>>(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var games = _gameService.GetAll();

            var response = games
                .Select(ToResponse)
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
            var game = _gameService.GetById(id);
            return game == null
                ? NotFound()
                : Ok(ToResponse(game));
        }

        [HttpPost]
        [EndpointSummary("Create a game")]
        [EndpointDescription("Adds a game and returns the created resource with its location.")]
        [ProducesResponseType<GameResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        public IActionResult Create(CreateGameRequest request)
        {
            var game = new Game
            {
                Title = request.Title,
                Genre = request.Genre,
                ReleaseYear = request.ReleaseYear
            };

            var createdGame = _gameService.Create(game);
            var response = ToResponse(createdGame);
            return CreatedAtRoute(nameof(GetById), new { id = createdGame.Id }, response);
        }

        [HttpPut("{id:int}")]
        [EndpointSummary("Replace a game")]
        [EndpointDescription("Replaces all editable fields on an existing game.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public IActionResult Replace(int id, UpdateGameRequest request)
        {
            var game = new Game
            {
                Title = request.Title,
                Genre = request.Genre,
                ReleaseYear = request.ReleaseYear
            };

            return _gameService.Replace(id, game) ? NoContent() : NotFound();
        }

        [HttpPatch("{id:int}")]
        [EndpointSummary("Update part of a game")]
        [EndpointDescription("Changes only the supplied fields on an existing game.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public IActionResult UpdatePartially(int id, PatchGameRequest request)
        {
            if (request.Title is null && request.Genre is null && request.ReleaseYear is null)
            {
                ModelState.AddModelError(nameof(request), "At least one field must be supplied.");
                return ValidationProblem(ModelState);
            }

            return _gameService.UpdatePartially(
                id,
                request.Title,
                request.Genre,
                request.ReleaseYear)
                ? NoContent()
                : NotFound();
        }

        [HttpDelete("{id:int}")]
        [EndpointSummary("Delete a game")]
        [EndpointDescription("Removes a game from the collection.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            return _gameService.Delete(id) ? NoContent() : NotFound();
        }

        private static GameResponse ToResponse(Game game) =>
           new(game.Id, game.Title, game.Genre, game.ReleaseYear);
    }
}
