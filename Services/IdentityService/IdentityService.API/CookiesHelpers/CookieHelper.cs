

namespace IdentityService.API.CookiesHelpers
{
    public static class CookieHelper
    {
        // appends the refresh token cookie
        public static void AppendRefreshTokenCookie(HttpResponse response, string refreshToken)
        {
            response.Cookies.Append("refreshToken", refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Path = "/api/v1/auth",
                    Expires = DateTimeOffset.UtcNow.AddDays(15)
                });
        }

        // Removes the refresh token cookie
        public static void DeleteRefreshTokenCookie(HttpResponse response)
        {
            response.Cookies.Delete("refreshToken", new CookieOptions { Path = "/api/v1/auth" });
        }
    }
}
