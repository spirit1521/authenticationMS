using System;
using System.Collections.Generic;
using System.Text;

namespace AuthMicroservice.Application.DTO
{
    public class TwoFactorSetupDto
    {
        public string? ManualEntryKey { get; set; }
        public string? QrCode { get; set; }
    }
}
