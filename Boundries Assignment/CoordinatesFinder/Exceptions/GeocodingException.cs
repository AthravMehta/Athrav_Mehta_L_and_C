namespace CoordinatesFinder.Exceptions
{
    public class GeocodingException : Exception
    {
        public GeocodingException()
        {
        }

        public GeocodingException(string message) : base(message)
        {
        }
    }
}
