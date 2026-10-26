using System;
using System.Collections.Generic;
using System.Text;

namespace AuthMicroservice.Application.DTO
{
    public class LoginResponseDto
    {
        public string? Message { get; set; }
        public string? Token { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public string? RefreshToken { get; set; }
    }
}
