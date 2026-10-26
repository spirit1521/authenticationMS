using AuthMicroservice.Application.DTO;
using AuthMicroservice.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AuthMicroservice.API.Controllers
{
    [Route("api/v1/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        IRoleService service;

        public RolesController(IRoleService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleDto dto)
        {
            var msg = await service.CreateRole(dto);
            return Ok(new{ msg = "Role created successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> FetchRoles()
        {
            return Ok(await service.FetchRoles());
        }

        [HttpPut("{id}/permissions")]
        public async Task<IActionResult> AssignPermissions(int id, AssignPermissionsDto dto)
        {
            return Ok(await service.AssignPermissions(id, dto));
        }
    }
}
