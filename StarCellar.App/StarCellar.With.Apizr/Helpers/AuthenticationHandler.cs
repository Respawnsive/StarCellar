using Apizr;
using Apizr.Authenticating;
using Apizr.Configuring.Manager;
using StarCellar.With.Apizr.Services.Apis.User;
using StarCellar.With.Apizr.Services.Apis.User.Dtos;

namespace StarCellar.With.Apizr.Helpers
{
    public class AuthenticationHandler<TWebApi> : AuthenticationHandlerBase
    {
        private readonly ISecureStorage _secureStorage;
        private readonly Lazy<IApizrManager<IUserApi>> _userManager;

        /// <inheritdoc />
        public AuthenticationHandler(
            IApizrManagerOptions<TWebApi> apizrOptions, 
            ISecureStorage secureStorage,
            Lazy<IApizrManager<IUserApi>> userManager) : base(apizrOptions)
        {
            _secureStorage = secureStorage;
            _userManager = userManager;
        }

        /// <inheritdoc />
        public override Task<string> GetTokenAsync(HttpRequestMessage request, CancellationToken ct = default) =>
            _secureStorage.GetAsync(nameof(Tokens.AccessToken));

        /// <inheritdoc />
        public override Task SetTokenAsync(HttpRequestMessage request, string token, CancellationToken ct = default) =>
            _secureStorage.SetAsync(nameof(Tokens.AccessToken), token);

        /// <inheritdoc />
        public override async Task<string> RefreshTokenAsync(HttpRequestMessage request, string token, CancellationToken ct = default)
        {
            // Do we have an access token?
            if (!string.IsNullOrWhiteSpace(token))
            {
                // Yes we have one. Do we have a refresh token too?
                var refreshToken = await _secureStorage.GetAsync(nameof(Tokens.RefreshToken));
                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    // Yes we have both. Let's try to refresh the token.
                    var tokens = new Tokens(token, refreshToken);
                    tokens = await _userManager.Value.ExecuteAsync(api => api.RefreshAsync(tokens));
                    if (!string.IsNullOrWhiteSpace(tokens.AccessToken) && !string.IsNullOrWhiteSpace(tokens.RefreshToken))
                    {
                        // Save the new tokens.
                        await _secureStorage.SetAsync(nameof(Tokens.AccessToken), tokens.AccessToken);
                        await _secureStorage.SetAsync(nameof(Tokens.RefreshToken), tokens.RefreshToken);

                        // Return the new access token to the previous request.
                        return tokens.AccessToken;
                    }
                }
            }

            // No access token or no refresh token or no success in refreshing the token.
            // Just return null to let the previous request fail as Unauthorized.
            return null;
        }
    }
}
