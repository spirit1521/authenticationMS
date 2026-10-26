using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthMicroservice.Application.DTO
{
    public class ForgotPasswordDto
    {
        [Required]
        public string? Username { get; set; }
    }
}
