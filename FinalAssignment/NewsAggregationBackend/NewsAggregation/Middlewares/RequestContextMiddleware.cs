using NewsAggregation.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace NewsAggregation.Middlewares
{
    public class RequestContextMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, RequestContext requestContext)
        {
            var user = httpContext.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (int.TryParse(userIdString, out var userId))
                {
                    requestContext.UserId = userId;
                }

                requestContext.Roles = user.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                requestContext.Email = user.FindFirstValue(ClaimTypes.Email);
            }

            await _next(httpContext);
        }
    }
}
