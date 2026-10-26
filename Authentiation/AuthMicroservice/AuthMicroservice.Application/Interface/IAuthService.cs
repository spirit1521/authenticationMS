using AuthMicroservice.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthMicroservice.Application.Interface
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Login(LoginDto dto);
        Task<string> Logout(int userId);
        Task<LoginResponseDto> RefreshToken(string refreshToken);
        Task<string> ChangePassword(int userId, ChangePasswordDto dto);
        Task<string> ForgotPassword(ForgotPasswordDto dto);
        Task<string> ResetPassword(ResetPasswordDto dto);

    }
}
