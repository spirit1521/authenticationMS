using AuthMicroservice.Application.DTO;
using AuthMicroservice.Application.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthMicroservice.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        IUserService service;

        public UsersController(IUserService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            var result = await service.CreateUser(dto);
            if (result == "Role not found")
            {
                return NotFound(new { msg = result });
            }

            if (result != "User created successfully")
            {
                return BadRequest(new { msg = result });
            }
            return StatusCode(201, new { msg = result });
        }

        [HttpGet]
        public async Task<IActionResult> FetchUsers()
        {
            var result = await service.FetchUsers();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FetchUserById(int id)
        {
            var result = await service.FetchUserById(id);
            if (result == null)
            {
                return NotFound(new { msg = "User not found" });
            }
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            var result = await service.UpdateUser(id, dto);
            if (result == "User not found" || result == "Role not found")
            {
                return NotFound(new { msg = result });
            }

            if (result != "User updated successfully")
            {
                return BadRequest(new { msg = result });
            }
            return Ok(new { msg = result });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await service.DeleteUser(id);
            if (result == "User not found")
            {
                return NotFound(new { msg = result });
            }
            return Ok(new { msg = result });
        }

        [HttpPut("{id}/lock")]
        public async Task<IActionResult> LockUser(int id)
        {
            var result = await service.LockUser(id);
            if (result == "User not found")
            {
                return NotFound(new { msg = result });
            }

            if (result != "User locked successfully")
            {
                return BadRequest(new { msg = result });
            }
            return Ok(new { msg = result });
        }

        [HttpPut("{id}/unlock")]
        public async Task<IActionResult> UnlockUser(int id)
        {
            var result = await service.UnlockUser(id);
            if (result == "User not found")
            {
                return NotFound(new { msg = result });
            }

            if (result != "User unlocked successfully")
            {
                return BadRequest(new { msg = result });
            }
            return Ok(new { msg = result });
        }
    }
}