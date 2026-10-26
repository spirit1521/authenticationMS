using AutoMapper;
using AuthMicroservice.Application.DTO;
using AuthMicroservice.Application.Interface;
using AuthMicroservice.Domain.Entities;
using AuthMicroservice.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthMicroservice.Infrastructure.Service
{
    public class UserService : IUserService
    {
        AppDbContext db;
        IMapper mapper;
        public UserService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<string> CreateUser(CreateUserDto dto)
        {
            string username = dto.Username;

            var existingUser = await db.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (existingUser != null)
            {
                return "Username already exists";
            }

            var role = await db.Roles.FindAsync(dto.RoleId);
            if (role == null)
            {
                return "Role not found";
            }

            var user = mapper.Map<User>(dto);

            user.Username = username;
            user.Email = dto.Email;
            user.MobileNumber = dto.MobileNumber;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.IsTwoFactorEnabled = false;
            user.TwoFactorSecretKey = null;

            db.Users.Add(user);
            await db.SaveChangesAsync();
            return "User created successfully";
        }

        public async Task<List<UserResponseDto>> FetchUsers()
        {
            var data = await db.Users
                .AsNoTracking()
                .Include(x => x.Role)
                .OrderBy(x => x.UserId)
                .ToListAsync();

            var result = mapper.Map<List<UserResponseDto>>(data);
            return result;
        }

        public async Task<UserResponseDto?> FetchUserById(int userId)
        {
            var data = await db.Users
                .AsNoTracking()
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (data == null)
            {
                return null;
            }

            var result = mapper.Map<UserResponseDto>(data);
            return result;
        }

        public async Task<string> UpdateUser(int userId, UpdateUserDto dto)
        {
            var user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                return "User not found";
            }

            if (user.Status == "Locked")
            {
                return "Unlock user before updating";
            }

            string username = dto.Username;
            var existingUser = await db.Users.FirstOrDefaultAsync(x =>x.Username == username && x.UserId != userId);
            if (existingUser != null)
            {
                return "Username already exists";
            }

            var role = await db.Roles.FindAsync(dto.RoleId);
            if (role == null)
            {
                return "Role not found";
            }

            mapper.Map(dto, user);

            user.Username = username;
            user.Email = dto.Email;
            user.MobileNumber = dto.MobileNumber;
            await db.SaveChangesAsync();
            return "User updated successfully";
        }

        public async Task<string> DeleteUser(int userId)
        {
            var user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                return "User not found";
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return "User deleted successfully";
        }

        public async Task<string> LockUser(int userId)
        {
            var user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                return "User not found";
            }

            if (user.Status == "Locked")
            {
                return "User is already locked";
            }

            if (user.Status == "Inactive")
            {
                return "Inactive user cannot be locked";
            }

            user.Status = "Locked";
            await db.SaveChangesAsync();
            return "User locked successfully";
        }

        public async Task<string> UnlockUser(int userId)
        {
            var user = await db.Users.FindAsync(userId);
            if (user == null)
            {
                return "User not found";
            }

            if (user.Status != "Locked")
            {
                return "User is not locked";
            }

            user.Status = "Active";
            await db.SaveChangesAsync();
            return "User unlocked successfully";
        }
    }
}