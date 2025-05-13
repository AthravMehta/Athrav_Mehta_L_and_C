using CoordinatesFinder.Constants;
using CoordinatesFinder.Exceptions;
using CoordinatesFinder.Helpers.Contracts;
using CoordinatesFinder.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace CoordinatesFinder.Helpers
{
    public class CoordinatesFinderHelper : ICoordinatesFinderHelper
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        public CoordinatesFinderHelper(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
        }
        public async Task<Coordinates> GetCoordinatesFromLocationAsync(string location)
        {
            string encodedLocation= Uri.EscapeDataString(location);
            string apiUrl = string.Format(AppConstants.BaseAddress, encodedLocation, configuration[$"{AppConstants.APIKeys}:{AppConstants.GeocodingAPIKey}"]);

            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
                throw new GeocodingException(string.Format(ErrorConstants.ApiCallFaildMessage, response.StatusCode));

            string responseString = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var places = JsonSerializer.Deserialize<List<LocationModel>>(responseString, options);

            var firstPlace = places?.FirstOrDefault();

            if (firstPlace == null)
                throw new GeocodingException(string.Format(ErrorConstants.NoResultFoundMessage, location));

            if (!double.TryParse(firstPlace.lat, out double latitude) || !double.TryParse(firstPlace.lon, out double longitude))
                throw new GeocodingException(ErrorConstants.LatLonParsingFailedMessage);

            return new Coordinates(latitude, longitude);
        }

    }
}
