using System.Security.Claims;

namespace UsefulWebApps.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static string? GetUserName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Name);
        }

        public static string? GetUserRole(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Role);
        }
    }
}
