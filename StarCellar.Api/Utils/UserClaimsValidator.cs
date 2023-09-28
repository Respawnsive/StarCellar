using System.Security.Claims;
using StarCellar.Api.Data;

namespace StarCellar.Api.Utils
{
    public static class UserClaimsValidator
    {
        public static bool TryValidate(ClaimsPrincipal claims, out User user, out string errorMessage)
        {
            if (!Guid.TryParse(claims.FindFirstValue("jti"), out var _))
                errorMessage = "Invalid `tokenId`: Not a valid GUID.";

            var userId = Guid.Empty;
            var username = "";
            user = null;
            errorMessage = "";

            var userIds = claims.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).ToArray();
            if (userIds.Length == 2) // only userid and username
            {
                if (Guid.TryParse(userIds[0].Value, out var userId0))
                {
                    userId = userId0;
                    username = userIds[1].Value;
                }
                else if (Guid.TryParse(userIds[1].Value, out var userId1))
                {
                    userId = userId1;
                    username = userIds[0].Value;
                }
                else
                {
                    errorMessage = "Invalid userid";
                    return false;
                }
            }
            else if (userIds.Length == 1) // only userid or only username
            {
                if (Guid.TryParse(userIds[0].Value, out var parsedUserId)) userId = parsedUserId;
                else username = userIds[0].Value;
            }
            else
            {
                errorMessage = "Invalid claims of type 'nameidentifier'";
                return false;
            }

            var fullName = claims.FindFirstValue(ClaimTypes.Name)!;
            var email = claims.FindFirstValue(ClaimTypes.Email)!;
            var role = claims.FindFirstValue(ClaimTypes.Role)!;

            user = new User
            {
                Id = userId,
                UserName = username,
                FullName = fullName,
                Email = email,
                Role = role
            };

            return true;
        }
    }
}
