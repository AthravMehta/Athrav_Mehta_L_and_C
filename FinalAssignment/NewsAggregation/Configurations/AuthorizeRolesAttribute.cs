using Microsoft.AspNetCore.Authorization;

namespace NewsAggregation.Configurations
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles) 
        {
            Roles = string.Join(",", roles);
        }
    }
}
