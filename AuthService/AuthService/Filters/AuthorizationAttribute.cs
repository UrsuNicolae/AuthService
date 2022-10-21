using AuthService.Models;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthService.Filters
{
    public class AuthorizationAttribute : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.IsAuthenticated())
            {
                var response = new ErrorModel()
                {
                    success = false,
                    error = "User is not authenticated or an invalid token was set."
                };

                context.HttpContext.Response.ContentType = "application/json";
                context.Result = new ObjectResult(response);
            };
        }
    }
}
