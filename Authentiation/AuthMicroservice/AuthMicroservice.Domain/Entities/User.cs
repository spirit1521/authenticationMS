using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AuthMicroservice.Domain.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public int BranchId { get; set; }
        [Required]
        public string Status { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public string? TwoFactorSecretKey { get; set; }
        public long? LastTwoFactorTimeStep { get; set; }
        public string? RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public string? PasswordResetTokenHash { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public int TokenVersion { get; set; }
        public DateTime CreatedAt { get; set; }


    }
}
