using AutoMapper;
using AuthMicroservice.Application.DTO;
using AuthMicroservice.Application.Interface;
using AuthMicroservice.Domain.Entities;
using AuthMicroservice.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthMicroservice.Infrastructure.Service
{
    public class RoleService : IRoleService
    {
        AppDbContext db;
        IMapper mapper;

        public RoleService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<string> CreateRole(CreateRoleDto dto)
        {
            string name = dto.RoleName;
            var existingRole = await db.Roles.FirstOrDefaultAsync(x => x.RoleName == name);

            if (existingRole != null)
            {
                return "Role already exists";
            }

            var role = mapper.Map<Role>(dto);
            role.RoleName = name;
            db.Roles.Add(role);
            await db.SaveChangesAsync();
            return "Role added successfully";
        }

        public async Task<List<RoleResponseDto>> FetchRoles()
        {
            var data = await db.Roles.Include(x => x.RolePermissions).ThenInclude(x => x.Permission)
                .OrderBy(x => x.RoleId).ToListAsync();

            var result = mapper.Map<List<RoleResponseDto>>(data);
            return result;
        }

        public async Task<string> AssignPermissions(int roleId, AssignPermissionsDto dto)
        {
            var role = await db.Roles.FindAsync(roleId);
            if (role == null)
            {
                return "Role not found";
            }

            var permissionIds = dto.PermissionIds.Distinct().ToList();
            var permissionCount = await db.Permissions.CountAsync(x => permissionIds.Contains(x.PermissionId));

            if (permissionCount != permissionIds.Count)
            {
                return "One or more permissions do not exist";
            }

            using var transaction = await db.Database.BeginTransactionAsync();
            var existingPermissions = await db.RolePermissions.Where(x => x.RoleId == roleId).ToListAsync();
            db.RolePermissions.RemoveRange(existingPermissions);
            await db.SaveChangesAsync();

            foreach (var permissionId in permissionIds)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                };

                db.RolePermissions.Add(rolePermission);
            }

            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            return "Permissions assigned successfully";
        }
    }
}