using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthMicroservice.Application.DTO
{
    public class AssignPermissionsDto
    {
        [Required]
        public List<int>? PermissionIds { get; set; } 

    }
}
