namespace GameCollectionApi.Services
{
    public interface IApiKeyService
    {
        Task<string> CreateAsync(string clientName);
        Task<bool> ValidateAsync(string apiKey);
    }
}
