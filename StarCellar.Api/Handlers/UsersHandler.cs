using Microsoft.AspNetCore.Identity;
using StarCellar.Api.Data;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using StarCellar.Api.Utils;
using static Microsoft.AspNetCore.Http.Results;

namespace StarCellar.Api.Handlers
{
    public static class UsersHandler
    {
        internal static async Task<IResult> SignUpAsync(
            AppDbContext appContext, 
            UserManager<User> userManager, 
            UserCreateDTO user
            )
        {
            if (user is null) return BadRequest();
            if (!MiniValidator.TryValidate(user, out var errors)) 
                return BadRequest(errors);

            if (userManager.Users.Any(u => u.Email == user.Email))
                return Conflict("Invalid `email`: A user with this email address already exists.");

            if (string.IsNullOrWhiteSpace(user.Username))
            {
                user.Username = string.Join('_', user.FullName.Split(' ')).ToLower();
                if (userManager.Users.Any(u => u.UserName == user.Username))
                    user.Username = user.Username + '_' + Guid.NewGuid().ToString("N")[..4];
            }
            else if (userManager.Users.Any(u => u.UserName == user.Username))
                return Conflict("Invalid `username`: A user with this username already exists.");

            var newUser = new User(user);
            var result = await userManager.CreateAsync(newUser, user.Password);

            var userDto = new UserDTO(newUser);

            return result.Succeeded ? Created($"/users/{userDto.Id}", userDto) : BadRequest(result.Errors);
        }

        internal static async Task<IResult> SignInAsync
        (
            UserManager<User> userManager,
            TokenGenerator tokenGenerator,
            UserRefreshTokenDbContext tokenContext,
            UserLoginDTO credentials,
            HttpResponse response
        )
        {
            if (credentials is null) return BadRequest();
            if (!MiniValidator.TryValidate(credentials, out var errors)) return BadRequest(errors);

            var isUsingEmailAddress = EmailAddressValidator.TryValidate(credentials.Login, out var _);
            var user = isUsingEmailAddress
                ? await userManager.FindByEmailAsync(credentials.Login)
                : await userManager.FindByNameAsync(credentials.Login);
            if (user is null) return NotFound("User with this email address not found.");

            var result = await userManager.CheckPasswordAsync(user, credentials.Password);
            if (!result) return BadRequest("Incorrect password.");

            var accessToken = tokenGenerator.GenerateAccessToken(user);
            var (refreshTokenId, refreshToken) = tokenGenerator.GenerateRefreshToken();

            var token = await tokenContext.UserRefreshTokens.Where(token => token.UserId == user.Id).FirstOrDefaultAsync();
            if(token is not null)
                tokenContext.UserRefreshTokens.Remove(token);

            await tokenContext.UserRefreshTokens.AddAsync(new UserRefreshToken { Id = refreshTokenId, UserId = user.Id });
            await tokenContext.SaveChangesAsync();

            var tokens = new Tokens(accessToken, refreshToken);

            return Ok(tokens);
        }

        internal static async Task<IResult> RefreshTokenAsync
        (
            UserRefreshTokenDbContext tokenContext,
            TokenValidator tokenValidator,
            TokenGenerator tokenGenerator,
            AppDbContext appContext,
            Tokens tokens,
            HttpResponse response
        )
        {
            if (string.IsNullOrWhiteSpace(tokens.AccessToken))
                return BadRequest("Please include an access token in the request.");

            if (string.IsNullOrWhiteSpace(tokens.RefreshToken))
                return BadRequest("Please include a refresh token in the request.");

            var accessTokenIsValid = tokenValidator.TryValidateAccessToken(tokens.AccessToken, out var claims, false);
            if (!accessTokenIsValid) return BadRequest("Invalid access token.");

            var refreshTokenIsValid = tokenValidator.TryValidateRefreshToken(tokens.RefreshToken, out var refreshTokenId);
            if (!refreshTokenIsValid) return BadRequest("Invalid refresh token.");

            var token = await tokenContext.UserRefreshTokens.Where(userRefreshToken => userRefreshToken.Id == refreshTokenId).FirstOrDefaultAsync();
            if (token is null) return BadRequest("Refresh token not found.");

            var user = await appContext.Users.Where(u => u.Id == token.UserId).FirstOrDefaultAsync();
            if (user is null || user.UserName != claims.Identity!.Name) return BadRequest("User not found.");

            var newAccessToken = tokenGenerator.GenerateAccessToken(user);
            var (newRefreshTokenId, newRefreshToken) = tokenGenerator.GenerateRefreshToken();

            tokenContext.UserRefreshTokens.Remove(token);
            await tokenContext.UserRefreshTokens.AddAsync(new UserRefreshToken { Id = newRefreshTokenId, UserId = user.Id });
            await tokenContext.SaveChangesAsync();

            var newTokens = new Tokens(newAccessToken, newRefreshToken);

            return Ok(newTokens);
        }

        internal static async Task<IResult> GetProfileAsync(
            IHttpContextAccessor httpContextAccessor,
            AppDbContext appContext)
        {
            if (!UserClaimsValidator.TryValidate(httpContextAccessor.HttpContext?.User, out var user, out var errMsg))
                return BadRequest(errMsg);

            var dbUser = await appContext.Users.FindAsync(user.Id);
            return dbUser == null ? NotFound() : Ok(dbUser);

        }

        internal static async Task<IResult> SignOutAsync
        (
            HttpRequest request,
            HttpResponse response,
            UserRefreshTokenDbContext tokenContext,
            Tokens tokens,
            TokenValidator tokenValidator
        )
        {
            if (string.IsNullOrWhiteSpace(tokens.RefreshToken))
                return BadRequest("Please include a refresh token in the request.");

            var refreshTokenIsValid = tokenValidator.TryValidateRefreshToken(tokens.RefreshToken, out var refreshTokenId, false);
            if (!refreshTokenIsValid) return BadRequest("Invalid refresh token.");

            var token = await tokenContext.UserRefreshTokens.Where(userRefreshToken => userRefreshToken.Id == refreshTokenId).FirstOrDefaultAsync();
            if (token is null) return BadRequest("Refresh token not found.");

            tokenContext.UserRefreshTokens.Remove(token);
            await tokenContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
