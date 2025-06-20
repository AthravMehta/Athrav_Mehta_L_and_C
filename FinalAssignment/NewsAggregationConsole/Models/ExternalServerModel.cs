namespace NewsAggregationConsole.Models
{
    public class ExternalServerDto
    {
        public int? ExternalServerId { get; set; }
        public string ServerName { get; set; }
        public string BaseUrl { get; set; }
        public string ApiKeyHash { get; set; }
        public bool IsActive { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
