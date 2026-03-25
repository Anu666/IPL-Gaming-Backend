using IPL.Gaming.Common.Enums;
using IPL.Gaming.Common.Models.CosmosDB;
using Microsoft.AspNetCore.Mvc;

namespace IPL.Gaming.Controllers
{
    /// <summary>
    /// Base controller with helper methods to access authenticated user
    /// </summary>
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Get the authenticated user from HttpContext
        /// </summary>
        protected User? CurrentUser => HttpContext.Items["User"] as User;

        /// <summary>
        /// Get the authenticated user's ID
        /// </summary>
        protected Guid? CurrentUserId => HttpContext.Items["UserId"] as Guid?;

        /// <summary>
        /// Get the authenticated user's role
        /// </summary>
        protected UserRole? CurrentUserRole => HttpContext.Items["UserRole"] as UserRole?;

        /// <summary>
        /// Check if current user is admin
        /// </summary>
        protected bool IsAdmin => CurrentUserRole == UserRole.Admin;

        /// <summary>
        /// Check if current user is moderator or admin
        /// </summary>
        protected bool IsModerator => CurrentUserRole == UserRole.Moderator || CurrentUserRole == UserRole.Admin;
    }
}
