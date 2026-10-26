using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthMicroservice.Application.DTO
{
    public class ResetPasswordDto
    {
        [Required]
        public string? Username { get; set; } 
        [Required]
        public string? Token { get; set; } 
        [Required]
        public string? NewPassword { get; set; } 
        [Required]
        public string? ConfirmPassword { get; set; } 
    }
}
