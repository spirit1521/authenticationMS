using AuthMicroservice.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthMicroservice.Application.Interface
{
    public interface ITwoFactorService
    {
        Task<TwoFactorSetupDto?> Setup(int userId);
        Task<bool> Enable(int userId, string code);
        Task<bool> VerifyCode(int userId, string code);
    }
}
