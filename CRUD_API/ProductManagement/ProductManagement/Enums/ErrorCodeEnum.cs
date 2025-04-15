namespace ProductManagement.Enums
{
    public enum ErrorCode
    {
        BadRequest = 400,
        NotFound = 404,
        Unauthorized = 401,
        Forbidden = 403,
        InternalServerError = 500,
        RequestEntityTooLarge = 413,
    }
}
