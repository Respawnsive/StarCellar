using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StarCellar.Api.Data;

namespace StarCellar.Api.Utils
{
    public class TokenValidator
    {
        private readonly byte[] _accessTokenSecret;
        private readonly byte[] _refreshTokenSecret;
        private readonly string _issuer;
        private readonly string _audience;

        public TokenValidator(IConfiguration config)
        {
            _accessTokenSecret = Encoding.ASCII.GetBytes(config.GetValue<string>("Jwt:AccessTokenSecret")!);
            _refreshTokenSecret = Encoding.ASCII.GetBytes(config.GetValue<string>("Jwt:RefreshTokenSecret")!);
            _issuer = config.GetValue<string>("Jwt:Issuer")!;
            _audience = config.GetValue<string>("Jwt:Audience")!;
        }

        public bool TryValidateAccessToken(string accessToken, out ClaimsPrincipal claims, bool validateLifetime = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_accessTokenSecret),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = validateLifetime,
                ClockSkew = TimeSpan.Zero,
                ValidAudience = _audience,
                ValidIssuer = _issuer,
            };

            try
            {
                var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParams, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    claims = null;
                    return false;
                }

                claims = principal;
                return claims != null;
            }
            catch (Exception)
            {
                claims = null;
                return false;
            }
        }

        public bool TryValidateRefreshToken(string refreshToken, out Guid tokenId, bool validateLifetime = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_refreshTokenSecret),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = validateLifetime,
                ClockSkew = TimeSpan.Zero,
                ValidAudience = _audience,
                ValidIssuer = _issuer,
            };

            try
            {
                tokenHandler.ValidateToken(refreshToken, tokenValidationParams, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    tokenId = Guid.Empty;
                    return false;
                }

                var valid = Guid.TryParse(jwtSecurityToken.Id, out var id);
                tokenId = id;

                return valid;
            }
            catch (Exception)
            {
                tokenId = Guid.Empty;
                return false;
            }
        }
    }
}
