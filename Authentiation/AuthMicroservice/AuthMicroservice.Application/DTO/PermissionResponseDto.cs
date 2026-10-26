using System;
using System.Collections.Generic;
using System.Text;

namespace AuthMicroservice.Application.DTO
{
    public class PermissionResponseDto
    {
        public int PermissionId { get; set; }
        public string? PermissionName { get; set; } 
        public string? ModuleName { get; set; } 
        public string? ActionType { get; set; }
        public string Message { get; set; }
    }
}
