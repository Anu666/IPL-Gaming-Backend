using IPL.Gaming.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IPL.Gaming.Attributes
{
    /// <summary>
    /// Attribute to restrict access based on user roles
    /// Must be used after ApiKeyAttribute to ensure user is authenticated
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly UserRole[] _allowedRoles;

        /// <summary>
        /// Initialize with one or more allowed roles
        /// </summary>
        public RequireRoleAttribute(params UserRole[] allowedRoles)
        {
            _allowedRoles = allowedRoles ?? Array.Empty<UserRole>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if user was authenticated by ApiKeyAttribute
            if (!context.HttpContext.Items.ContainsKey("User") || 
                !context.HttpContext.Items.ContainsKey("UserRole"))
            {
                context.Result = new UnauthorizedObjectResult(new 
                { 
                    message = "Authentication required. Please provide a valid API key." 
                });
                return;
            }

            var userRole = context.HttpContext.Items["UserRole"] as UserRole?;

            if (userRole == null)
            {
                context.Result = new UnauthorizedObjectResult(new 
                { 
                    message = "User role not found." 
                });
                return;
            }

            // Check if user's role is in the allowed roles
            if (_allowedRoles.Length > 0 && !_allowedRoles.Contains(userRole.Value))
            {
                var allowedRoleNames = string.Join(", ", _allowedRoles.Select(r => r.ToString()));
                context.Result = new ObjectResult(new 
                { 
                    message = $"Access denied. This endpoint requires one of the following roles: {allowedRoleNames}",
                    requiredRoles = allowedRoleNames,
                    yourRole = userRole.Value.ToString()
                })
                {
                    StatusCode = 403 // Forbidden
                };
                return;
            }
        }
    }
}
