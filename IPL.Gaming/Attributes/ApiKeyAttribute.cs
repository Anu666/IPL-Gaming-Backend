using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IPL.Gaming.Attributes
{
    /// <summary>
    /// Attribute to validate API key in request headers
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute, IAuthorizationFilter
    {
        private const string API_KEY_HEADER_NAME = "X-Api-Key";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if API key is present in headers
            if (!context.HttpContext.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var apiKeyHeaderValue))
            {
                context.Result = new UnauthorizedObjectResult(new 
                { 
                    message = "API Key is missing. Please provide a valid API key in the X-Api-Key header." 
                });
                return;
            }

            var apiKey = apiKeyHeaderValue.ToString();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                context.Result = new UnauthorizedObjectResult(new 
                { 
                    message = "API Key is invalid." 
                });
                return;
            }

            // Get the user cache service
            var userCacheService = context.HttpContext.RequestServices.GetService<UserCacheService>();
            
            if (userCacheService == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            // Validate API key and get user
            var user = userCacheService.GetUserByApiKey(apiKey);

            if (user == null)
            {
                context.Result = new UnauthorizedObjectResult(new 
                { 
                    message = "Invalid API Key. Please provide a valid API key." 
                });
                return;
            }

            if (!user.IsActive)
            {
                context.Result = new UnauthorizedObjectResult(new 
                { 
                    message = "User account is inactive." 
                });
                return;
            }

            // Store user in HttpContext for later use in controllers
            context.HttpContext.Items["User"] = user;
            context.HttpContext.Items["UserId"] = user.Id;
            context.HttpContext.Items["UserRole"] = user.Role;
        }
    }
}
