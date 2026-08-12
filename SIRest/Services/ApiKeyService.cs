using GameCollectionApi.Data;
using GameCollectionApi.Models;
using GameCollectionApi.Repositories;

namespace GameCollectionApi.Services
{
    public class ApiKeyService(IApiClientRepository repository) : IApiKeyService
    {
        public async Task<string> CreateAsync(string clientName)
        {
            string apiKey = ApiKeyHelper.Generate();

            var apiClient = new ApiClient
            {
                Name = clientName,
                ApiKeyHash = ApiKeyHelper.Hash(apiKey),
                CreatedAtUtc = DateTime.UtcNow,
                IsActive = true
            };

            await repository.CreateAsync(apiClient);

            return apiKey;
        }

        public async Task<bool> ValidateAsync(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return false;
            }

            string apiKeyHash = ApiKeyHelper.Hash(apiKey);

            ApiClient? client =
                await repository.GetActiveByKeyHashAsync(apiKeyHash);

            return client is not null;
        }
    }
}
