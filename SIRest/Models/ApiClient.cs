namespace GameCollectionApi.Models
{
    public class ApiClient
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string ApiKeyHash { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public bool IsActive { get; set; } = true;
    }

}
