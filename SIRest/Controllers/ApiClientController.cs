using GameCollectionApi.DTO;
using GameCollectionApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameCollectionApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiClientsController : ControllerBase
    {
        private readonly IApiKeyService _apiKeyService;

        public ApiClientsController(IApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }

        [HttpPost]
        public async Task<ActionResult<CreateApiClientResponse>> Create(
            CreateApiClientRequest request)
        {
            string apiKey =
                await _apiKeyService.CreateAsync(request.Name);

            var response = new CreateApiClientResponse(
                request.Name,
                apiKey);

            return Created(string.Empty, response);
        }
    }
}
