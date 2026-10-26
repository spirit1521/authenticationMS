using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthMicroservice.Domain.Entities
{
    public class Permission
    {
        [Key]
        public int PermissionId { get; set; }
        [Required]
        public string PermissionName { get; set; } = string.Empty;
        [Required]
        public string ModuleName { get; set; } = string.Empty;
        [Required]
        public string ActionType { get; set; } = string.Empty;
        public List<RolePermission>? RolePermissions { get; set; }
    }
}
