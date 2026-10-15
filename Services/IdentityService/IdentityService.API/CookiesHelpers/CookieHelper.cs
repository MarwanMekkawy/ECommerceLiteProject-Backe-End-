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
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddDays(15)
                });
        }

        // Removes the refresh token cookie
        public static void DeleteRefreshTokenCookie(HttpResponse response)
        {
            response.Cookies.Delete("refreshToken", new CookieOptions { Path = "/" });
        }
    }
}
