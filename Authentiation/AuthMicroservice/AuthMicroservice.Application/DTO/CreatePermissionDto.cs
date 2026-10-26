using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthMicroservice.Application.DTO
{
    public class CreatePermissionDto
    {
        [Required]
        public string? PermissionName { get; set; } 

        [Required]
        public string? ModuleName { get; set; } 

        [Required]
        public string? ActionType { get; set; } 
    }
}
