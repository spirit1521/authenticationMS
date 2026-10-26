using System;
using System.Collections.Generic;
using System.Text;

namespace AuthMicroservice.Application.DTO
{
    public class UserResponseDto
    {
        public int UserId { get; set; }

        public int EmployeeId { get; set; }

        public string? Username { get; set; } 

        public string? Email { get; set; } 

        public string? MobileNumber { get; set; }

        public int RoleId { get; set; }

        public string? RoleName { get; set; } 

        public int BranchId { get; set; }

        public string? Status { get; set; }

        public bool IsTwoFactorEnabled { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
