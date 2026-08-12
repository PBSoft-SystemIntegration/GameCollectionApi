using GameCollectionApi.Models;

namespace GameCollectionApi.Repositories
{
    public class InMemoryApiClientRepository : IApiClientRepository
    {
        private readonly List<ApiClient> apiClients = [];
        public Task<ApiClient> CreateAsync(ApiClient apiClient)
        {
            apiClient.Id = apiClients.Count + 1;
            apiClients.Add(apiClient);

            return Task.FromResult(apiClient);
        }

        public Task<ApiClient?> GetActiveByKeyHashAsync(string apiKeyHash)
        {
            ApiClient? client = apiClients.FirstOrDefault(client =>
                client.ApiKeyHash == apiKeyHash &&
                client.IsActive);

            return Task.FromResult(client);
        }
    }
}
