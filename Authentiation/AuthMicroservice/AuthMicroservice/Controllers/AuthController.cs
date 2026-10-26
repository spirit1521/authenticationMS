using AuthMicroservice.Application.DTO;
using AuthMicroservice.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AuthMicroservice.API.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService service;
        private readonly ITwoFactorService twoFactorService;
        private readonly JwtSettingsDto settings;

        public AuthController(IAuthService service, ITwoFactorService twoFactorService, IOptions<JwtSettingsDto> options)
        {
            this.service = service;
            this.twoFactorService = twoFactorService;
            this.settings = options.Value;
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await service.Login(dto);
            if (string.IsNullOrEmpty(result.Token))
            {
                return Unauthorized(result);
            }

            Response.Headers.CacheControl = "no-store";
            Response.Cookies.Append("access_token", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddMinutes(settings.ExpiryInMinutes)
            });

            Response.Cookies.Append("refresh_token", result.RefreshToken!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/v1/auth",
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(new { msg = "Login successful" });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await service.Logout(userId);
            Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
            Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/api/v1/auth" });
            return Ok(new { msg = result });
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var token = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { msg = "Refresh token is missing" });
            }

            var result = await service.RefreshToken(token);
            if (string.IsNullOrEmpty(result.Token))
            {
                Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
                Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/api/v1/auth" });
                return Unauthorized(new { msg = result.Message });
            }

            Response.Headers.CacheControl = "no-store";

            Response.Cookies.Append("access_token", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddMinutes(settings.ExpiryInMinutes)
            });

            Response.Cookies.Append("refresh_token", result.RefreshToken!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/v1/auth",
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
            return Ok(new { msg = "Token refreshed successfully" });
        }

        [Authorize]
        [EnableRateLimiting("auth")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await service.ChangePassword(userId, dto);
            if (result != "Password changed successfully. Please log in again")
            {
                return BadRequest(new { msg = result });
            }

            Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
            Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/api/v1/auth" });

            return Ok(new { msg = result });
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var result = await service.ForgotPassword(dto);
            return Ok(new { msg = result });
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var result = await service.ResetPassword(dto);
            if (result != "Password reset successfully. Please log in again")
            {
                return BadRequest(new { msg = result });
            }

            Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
            Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/api/v1/auth" });
            return Ok(new { msg = result });
        }

        [Authorize]
        [HttpPost("2fa/setup")]
        public async Task<IActionResult> SetupTwoFactor()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await twoFactorService.Setup(userId);
            if (result == null)
            {
                return BadRequest(new { msg = "Unable to set up 2FA" });
            }
            Response.Headers.CacheControl = "no-store";
            return Ok(result);
        }

        [Authorize]
        [EnableRateLimiting("auth")]
        [HttpPost("2fa/enable")]
        public async Task<IActionResult> EnableTwoFactor(TwoFactorCodeDto dto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await twoFactorService.Enable(userId, dto.Code);
            if (!result)
            {
                return BadRequest(new { msg = "Unable to enable 2FA" });
            }
            return Ok(new { msg = "2FA enabled successfully" });
        }
    }
}