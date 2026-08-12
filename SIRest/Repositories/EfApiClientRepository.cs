using GameCollectionApi.Data;
using GameCollectionApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameCollectionApi.Repositories
{
    public class EfApiClientRepository(GameDbContext context) : IApiClientRepository
    {
        public async Task<ApiClient> CreateAsync(ApiClient apiClient)
        {
            context.ApiClients.Add(apiClient);
            await context.SaveChangesAsync();

            return apiClient;
        }

        public async Task<ApiClient?> GetActiveByKeyHashAsync(string apiKeyHash)
        {
            return await context.ApiClients
           .AsNoTracking()
           .FirstOrDefaultAsync(client =>
               client.ApiKeyHash == apiKeyHash &&
               client.IsActive);
        }
    }
}
