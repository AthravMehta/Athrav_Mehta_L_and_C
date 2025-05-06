using CoordinatesFinder.Constants;
using CoordinatesFinder.Exceptions;
using CoordinatesFinder.Helpers;
using CoordinatesFinder.Models;
using Microsoft.Extensions.Configuration;


namespace CoordinatesFinder
{
    public class CoordinatesFinder
    {
        public static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(AppConstants.AppSettingsJson).Build();

            HttpClient client = new HttpClient();
            CoordinatesFinderHelper coordinatesFinder = new CoordinatesFinderHelper(client, configuration);

            try
            {
                var location = GetLocation();
                Coordinates coordinates = await coordinatesFinder.GetCoordinatesFromLocationAsync(location);
                Console.WriteLine($"Latitude: {coordinates.Latitude}, Longitude: {coordinates.Longitude}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static string GetLocation()
        {
            string input;
            do
            {
                Console.Write("Enter location to get Coordintates: ");
                input = Console.ReadLine()?.Trim();

            } while (!ValidateLocation(input));

            return input;
        }


        public static bool ValidateLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                Console.WriteLine("Entered Input Address is Empty");
                return false;
            }
            return true;
        }

    }
}
