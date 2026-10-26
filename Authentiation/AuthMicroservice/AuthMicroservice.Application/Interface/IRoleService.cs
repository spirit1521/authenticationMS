using AuthMicroservice.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthMicroservice.Application.Interface
{
    public interface IRoleService
    {
        Task<string> CreateRole(CreateRoleDto dto);
        Task<List<RoleResponseDto>> FetchRoles();
        Task<string> AssignPermissions(int roleId, AssignPermissionsDto dto);

    }
}
