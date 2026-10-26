using AuthMicroservice.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthMicroservice.Application.Interface
{
    public interface IPermissionService
    {
        Task<PermissionResponseDto> CreatePermission(CreatePermissionDto dto);

    }
}
