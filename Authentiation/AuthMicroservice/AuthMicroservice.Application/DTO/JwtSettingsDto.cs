using System;
using System.Collections.Generic;
using System.Text;

namespace AuthMicroservice.Application.DTO
{
    public class JwtSettingsDto
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int ExpiryInMinutes { get; set; }
    }
}
