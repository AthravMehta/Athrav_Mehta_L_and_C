using CoordinatesFinder.Models;

namespace CoordinatesFinder.Helpers.Contracts
{
    public interface ICoordinatesFinderHelper
    {
        Task<Coordinates> GetCoordinatesFromLocationAsync(string location);
    }
}
