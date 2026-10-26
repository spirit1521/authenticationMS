using System;
using System.Collections.Generic;
using System.Text;

namespace AuthMicroservice.Application.DTO
{
    public class RoleResponseDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<PermissionResponseDto>? Permissions { get; set; }
    }
}
