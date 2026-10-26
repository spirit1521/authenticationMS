using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthMicroservice.Application.DTO
{
    public class ChangePasswordDto
    {
        [Required]
        public string? CurrentPassword { get; set; } 

        [Required]
        public string? NewPassword { get; set; } 

        [Required]
        public string? ConfirmPassword { get; set; } 
    }
}
