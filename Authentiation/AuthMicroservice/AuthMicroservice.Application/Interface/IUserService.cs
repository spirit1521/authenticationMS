using AuthMicroservice.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthMicroservice.Application.Interface
{
    public interface IUserService
    {
        Task<string> CreateUser(CreateUserDto dto);
        Task<List<UserResponseDto>> FetchUsers();
        Task<UserResponseDto?> FetchUserById(int userId);
        Task<string> UpdateUser(int userId, UpdateUserDto dto);
        Task<string> DeleteUser(int userId);
        Task<string> LockUser(int userId);
        Task<string> UnlockUser(int userId);
    }
}