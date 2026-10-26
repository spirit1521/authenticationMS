using AuthMicroservice.Application.DTO;
using AuthMicroservice.Domain.Entities;
using AutoMapper;
using System.Data;
using System.Security;

namespace AuthMicroservice.Application.Mapper
{
    public class AuthMapper : Profile
    {
        public AuthMapper()
        {
            CreateMap<CreateRoleDto, Role>()
                .ForMember(x => x.RoleId, x => x.Ignore())
                .ForMember(x => x.RolePermissions, x => x.Ignore());

            CreateMap<Role, RoleResponseDto>()
                .ForMember(x => x.Permissions,
                    x => x.MapFrom(r => r.RolePermissions.Select(rp => rp.Permission)));

            CreateMap<CreatePermissionDto, Permission>()
                .ForMember(x => x.PermissionId, x => x.Ignore())
                .ForMember(x => x.RolePermissions, x => x.Ignore());

            CreateMap<Permission, PermissionResponseDto>();

            CreateMap<CreateUserDto, User>()
                .ForMember(x => x.UserId, x => x.Ignore())
                .ForMember(x => x.PasswordHash, x => x.Ignore())
                .ForMember(x => x.Role, x => x.Ignore())
                .ForMember(x => x.IsTwoFactorEnabled, x => x.Ignore())
                .ForMember(x => x.TwoFactorSecretKey, x => x.Ignore())
                .ForMember(x => x.CreatedAt, x => x.Ignore());

            CreateMap<UpdateUserDto, User>()
                .ForMember(x => x.UserId, x => x.Ignore())
                .ForMember(x => x.PasswordHash, x => x.Ignore())
                .ForMember(x => x.Role, x => x.Ignore())
                .ForMember(x => x.IsTwoFactorEnabled, x => x.Ignore())
                .ForMember(x => x.TwoFactorSecretKey, x => x.Ignore())
                .ForMember(x => x.CreatedAt, x => x.Ignore());

            CreateMap<User, UserResponseDto>()
                .ForMember(x => x.RoleName,
                    x => x.MapFrom(u => u.Role != null ? u.Role.RoleName : ""));
        }
    }
}