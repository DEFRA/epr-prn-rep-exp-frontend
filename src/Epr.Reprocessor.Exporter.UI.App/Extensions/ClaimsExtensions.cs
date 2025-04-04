using System.Security.Claims;

namespace Epr.Reprocessor.Exporter.UI.App.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetClaim(this IEnumerable<Claim> claims, string claimName)
        {
            var claimValue = claims?.ToList().Find(c => c.Type == claimName);

            return claimValue?.Value;
        }
    }
}
