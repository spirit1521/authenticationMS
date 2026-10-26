using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthMicroservice.Application.DTO
{
    public class CreateRoleDto
    {
        [Required]
        public string? RoleName { get; set; } 
        public string? Description { get; set; }

    }
}
