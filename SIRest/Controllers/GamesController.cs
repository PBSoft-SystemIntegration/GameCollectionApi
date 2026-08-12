using GameCollectionApi.Attributes;
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
        [ApiKey]
        [HttpGet]
        [EndpointSummary("Get all games")]
        [EndpointDescription("Returns every game in the collection.")]
        [ProducesResponseType<IReadOnlyCollection<GameResponse>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var games = await _gameService.GetAllAsync();

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
        public async Task<IActionResult> GetById(int id)
        {
            var game = await _gameService.GetByIdAsync(id);
            return game == null
                ? NotFound()
                : Ok(ToResponse(game));
        }

        [HttpPost]
        [EndpointSummary("Create a game")]
        [EndpointDescription("Adds a game and returns the created resource with its location.")]
        [ProducesResponseType<GameResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateGameRequest request)
        {
            var game = new Game
            {
                Title = request.Title,
                Genre = request.Genre,
                ReleaseYear = request.ReleaseYear
            };

            var createdGame = await _gameService.CreateAsync(game);
            var response = ToResponse(createdGame);
            return CreatedAtRoute(nameof(GetById), new { id = createdGame.Id }, response);
        }

        [HttpPut("{id:int}")]
        [EndpointSummary("Replace a game")]
        [EndpointDescription("Replaces all editable fields on an existing game.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Replace(int id, UpdateGameRequest request)
        {
            var game = new Game
            {
                Title = request.Title,
                Genre = request.Genre,
                ReleaseYear = request.ReleaseYear
            };

            return await _gameService.ReplaceAsync(id, game)
                ? NoContent()
                : NotFound();
        }

        [HttpPatch("{id:int}")]
        [EndpointSummary("Update part of a game")]
        [EndpointDescription("Changes only the supplied fields on an existing game.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePartially(int id, PatchGameRequest request)
        {
            if (request.Title is null && request.Genre is null && request.ReleaseYear is null)
            {
                ModelState.AddModelError(nameof(request), "At least one field must be supplied.");
                return ValidationProblem(ModelState);
            }

            return await _gameService.UpdatePartiallyAsync(
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
        public async Task<IActionResult> Delete(int id)
        {
            return await _gameService.DeleteAsync(id)
                ? NoContent()
                : NotFound();
        }

        private static GameResponse ToResponse(Game game) =>
           new(game.Id, game.Title, game.Genre, game.ReleaseYear);
    }
}
