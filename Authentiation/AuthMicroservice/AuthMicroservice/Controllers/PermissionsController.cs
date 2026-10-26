using AuthMicroservice.Application.DTO;
using AuthMicroservice.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AuthMicroservice.API.Controllers
{
    [Route("api/v1/permissions")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        IPermissionService service;

        public PermissionsController(IPermissionService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermission(CreatePermissionDto dto)
        {
            var msg = await service.CreatePermission(dto);
            return Ok(new{ msg = "Permission created successfully" });
        }
    }
}
