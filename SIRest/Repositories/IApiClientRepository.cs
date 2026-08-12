using GameCollectionApi.Models;

namespace GameCollectionApi.Repositories
{
    public interface IApiClientRepository
    {
        Task<ApiClient> CreateAsync(ApiClient apiClient);
        Task<ApiClient?> GetActiveByKeyHashAsync(string apiKeyHash);
    }
}
