using AuthMicroservice.Application.DTO;
using AuthMicroservice.Application.Interface;
using AuthMicroservice.Domain.Entities;
using AuthMicroservice.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthMicroservice.Infrastructure.Service
{
    public class AuthService : IAuthService
    {
        AppDbContext db;
        IJwtService jwt;
        ITwoFactorService tf;
        IEmailService emailService;

        public AuthService(AppDbContext db, IJwtService jwt, ITwoFactorService tf, IEmailService emailService)
        {
            this.db = db;
            this.jwt = jwt;
            this.tf = tf;
            this.emailService = emailService;
        }

        public async Task<LoginResponseDto> Login(LoginDto dto)
        {
            var user = await db.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Username == dto.Username.Trim());

            if (user == null)
            {
                return new LoginResponseDto
                {
                    Message = "Invalid username or password"
                };
            }

            if (Encoding.UTF8.GetByteCount(dto.Password) > 72)
            {
                return new LoginResponseDto
                {
                    Message = "Invalid username or password"
                };
            }

            bool passwordMatch = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!passwordMatch)
            {
                return new LoginResponseDto
                {
                    Message = "Invalid username or password"
                };
            }

            if (user.Status != "Active")
            {
                return new LoginResponseDto
                {
                    Message = "Account is inactive or locked"
                };
            }

            if (user.Role == null)
            {
                return new LoginResponseDto
                {
                    Message = "User role not found"
                };
            }

            if (user.IsTwoFactorEnabled)
            {
                if (string.IsNullOrWhiteSpace(dto.Code))
                {
                    return new LoginResponseDto
                    {
                        Message = "Authenticator code required",
                        RequiresTwoFactor = true
                    };
                }

                bool validCode = await tf.VerifyCode(user.UserId, dto.Code);

                if (!validCode)
                {
                    return new LoginResponseDto
                    {
                        Message = "Invalid or already used authenticator code",
                        RequiresTwoFactor = true
                    };
                }
            }

            return await CreateTokens(user);
        }

        public async Task<string> Logout(int userId)
        {
            await db.Users
                .Where(x => x.UserId == userId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(u => u.RefreshTokenHash, (string?)null)
                    .SetProperty(u => u.RefreshTokenExpiry, (DateTime?)null)
                    .SetProperty(u => u.TokenVersion, u => u.TokenVersion + 1));

            return "Logged out successfully";
        }

        public async Task<LoginResponseDto> RefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return new LoginResponseDto
                {
                    Message = "Refresh token is required"
                };
            }

            string hash = HashToken(refreshToken);

            var user = await db.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.RefreshTokenHash == hash &&
                    x.RefreshTokenExpiry > DateTime.UtcNow);

            if (user == null)
            {
                return new LoginResponseDto
                {
                    Message = "Invalid or expired refresh token"
                };
            }

            if (user.Status != "Active")
            {
                return new LoginResponseDto
                {
                    Message = "Account is inactive or locked"
                };
            }

            if (user.Role == null)
            {
                return new LoginResponseDto
                {
                    Message = "User role not found"
                };
            }

            return await CreateTokens(user, hash);
        }

        public async Task<string> ChangePassword(int userId, ChangePasswordDto dto)
        {
            var user = await db.Users.FindAsync(userId);

            if (user == null || user.Status != "Active")
            {
                return "User is unavailable";
            }

            if (Encoding.UTF8.GetByteCount(dto.CurrentPassword) > 72)
            {
                return "Current password is incorrect";
            }

            bool passwordMatch = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);

            if (!passwordMatch)
            {
                return "Current password is incorrect";
            }

            if (!ValidPassword(dto.NewPassword))
            {
                return "Password must be 8-64 characters and at most 72 UTF-8 bytes";
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return "Passwords do not match";
            }

            bool samePassword = BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.PasswordHash);

            if (samePassword)
            {
                return "New password must differ from current password";
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            int updated = await db.Users
                .Where(x => x.UserId == userId &&
                    x.Status == "Active" &&
                    x.TokenVersion == user.TokenVersion &&
                    x.PasswordHash == user.PasswordHash).ExecuteUpdateAsync(x => x
                    .SetProperty(u => u.PasswordHash, passwordHash)
                    .SetProperty(u => u.RefreshTokenHash, (string?)null)
                    .SetProperty(u => u.RefreshTokenExpiry, (DateTime?)null)
                    .SetProperty(u => u.PasswordResetTokenHash, (string?)null)
                    .SetProperty(u => u.PasswordResetTokenExpiry, (DateTime?)null)
                    .SetProperty(u => u.TokenVersion, u => u.TokenVersion + 1));

            if (updated == 0)
            {
                return "Account changed. Please try again";
            }

            return "Password changed successfully. Please log in again";
        }

        public async Task<string> ForgotPassword(ForgotPasswordDto dto)
        {
            string msg = "If the account is eligible, a reset token will be sent to its registered email";
            var user = await db.Users.FirstOrDefaultAsync(x => x.Username == dto.Username);

            if (user == null || user.Status != "Active" || string.IsNullOrWhiteSpace(user.Email))
            {
                return msg;
            }

            string token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            string hash = HashToken(token);
            DateTime expiry = DateTime.UtcNow.AddMinutes(15);

            int updated = await db.Users
                .Where(x => x.UserId == user.UserId &&
                    x.Status == "Active" &&
                    x.TokenVersion == user.TokenVersion).ExecuteUpdateAsync(x => x
                    .SetProperty(u => u.PasswordResetTokenHash, hash)
                    .SetProperty(u => u.PasswordResetTokenExpiry, expiry));

            if (updated == 1)
            {
                await emailService.SendResetToken(user.Email, token);
            }

            return msg;
        }

        public async Task<string> ResetPassword(ResetPasswordDto dto)
        {
            if (!ValidPassword(dto.NewPassword))
            {
                return "Password is required and must not exceed 72 UTF-8 bytes";
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return "Passwords do not match";
            }

            if (string.IsNullOrWhiteSpace(dto.Token))
            {
                return "Invalid or expired reset token";
            }

            string hash = HashToken(dto.Token);
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            int updated = await db.Users
                .Where(x => x.Username == dto.Username &&
                    x.Status == "Active" &&
                    x.PasswordResetTokenHash == hash &&
                    x.PasswordResetTokenExpiry > DateTime.UtcNow)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(u => u.PasswordHash, passwordHash)
                    .SetProperty(u => u.PasswordResetTokenHash, (string?)null)
                    .SetProperty(u => u.PasswordResetTokenExpiry, (DateTime?)null)
                    .SetProperty(u => u.RefreshTokenHash, (string?)null)
                    .SetProperty(u => u.RefreshTokenExpiry, (DateTime?)null)
                    .SetProperty(u => u.TokenVersion, u => u.TokenVersion + 1));

            if (updated == 0)
            {
                return "Invalid or expired reset token";
            }

            return "Password reset successfully. Please log in again";
        }

        private async Task<LoginResponseDto> CreateTokens(User user, string? oldRefreshHash = null)
        {
            string refreshToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            string hash = HashToken(refreshToken);
            DateTime expiry = DateTime.UtcNow.AddDays(7);

            int updated = await db.Users
                .Where(x => x.UserId == user.UserId &&
                    x.Status == "Active" &&
                    x.TokenVersion == user.TokenVersion &&
                    (oldRefreshHash == null ||
                        (x.RefreshTokenHash == oldRefreshHash &&
                         x.RefreshTokenExpiry > DateTime.UtcNow)))
                .ExecuteUpdateAsync(x => x
                    .SetProperty(u => u.RefreshTokenHash, hash)
                    .SetProperty(u => u.RefreshTokenExpiry, expiry)
                    .SetProperty(u => u.TokenVersion, u => u.TokenVersion + 1));

            if (updated == 0)
            {
                return new LoginResponseDto
                {
                    Message = "Session changed. Please log in again"
                };
            }

            user.TokenVersion++;

            return new LoginResponseDto
            {
                Message = "Authentication successful",
                Token = jwt.GenerateToken(user),
                RefreshToken = refreshToken
            };
        }

        private string HashToken(string token)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        }

        private bool ValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && Encoding.UTF8.GetByteCount(password) <= 72;
        }
    }
}