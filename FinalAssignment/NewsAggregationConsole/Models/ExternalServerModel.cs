namespace NewsAggregationConsole.Models
{
    public class ExternalServerDto
    {
        public Guid? Id { get; set; }
        public string ServerName { get; set; }
        public string BaseUrl { get; set; }
        public string ApiKeyHash { get; set; }
        public bool IsActive { get; set; }
    }
}
