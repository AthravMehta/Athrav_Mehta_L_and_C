namespace CoordinatesFinder.Models
{
    public class LocationModel
    {
        public string lat { get; set; }
        public string lon { get; set; }
        public string display_name { get; set; }
    }
    public record Coordinates(double Latitude, double Longitude);
}
