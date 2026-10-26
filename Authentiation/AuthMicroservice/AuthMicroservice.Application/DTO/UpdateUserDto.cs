using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthMicroservice.Application.DTO
{
    public class UpdateUserDto
    {
        public int EmployeeId { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string? Email { get; set; } 
        [Required]
        public string? MobileNumber { get; set; } 
        public int RoleId { get; set; }
        public int BranchId { get; set; }
        [Required]
        public string Status { get; set; } = "Active";
    }
}
