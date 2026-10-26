using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthMicroservice.Application.DTO
{
    public class TwoFactorCodeDto
    {
        [Required]
        public string? Code { get; set; }
    }
}
