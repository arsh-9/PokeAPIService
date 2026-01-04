
using Microsoft.AspNetCore.Mvc;
using PokeAPIService.Models.Auth;
namespace PokeAPIService.Controllers

{

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly IRefreshTokenStore _refreshStore;

        public AuthController(
            TokenService tokenService,
            IRefreshTokenStore refreshStore)
        {
            _tokenService = tokenService;
            _refreshStore = refreshStore;
        }

        [HttpPost("login")]
        public IActionResult Login()
        {
            // Demo user
            var userId = "test-user";

            var accessToken = _tokenService.CreateAccessToken(userId);
            var refreshToken = _tokenService.CreateRefreshToken();

            _refreshStore.Save(userId, refreshToken);

            Response.Cookies.Append(
                "refreshToken",
                refreshToken.Token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,          // HTTPS only
                    SameSite = SameSiteMode.Strict,
                    Path = "/api/auth/refresh",
                    Expires = refreshToken.ExpiresAt
                });

            return Ok(new AuthToken
            {
                AccessToken = accessToken
            });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh()
        {
            //Demo user
            var userId = "test-user";

            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken is null)
            {
                return Unauthorized("Invalid refresh token");
            }
            var stored = _refreshStore.Get(userId, refreshToken);
            if (stored == null || stored.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("Invalid refresh token");

            //Rotate token
            _refreshStore.Revoke(userId, refreshToken);

            var newRefreshToken = _tokenService.CreateRefreshToken();
            _refreshStore.Save(userId, newRefreshToken);

            Response.Cookies.Append(
                "refreshToken",
                newRefreshToken.Token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,          // HTTPS only
                    SameSite = SameSiteMode.Strict,
                    Path = "/api/auth/refresh",
                    Expires = newRefreshToken.ExpiresAt
                });

            return Ok(new AuthToken
            {
                AccessToken = _tokenService.CreateAccessToken(userId)
            });
        }
    }


}

