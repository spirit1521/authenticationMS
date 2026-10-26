using AuthMicroservice.Application.DTO;
using AuthMicroservice.Application.Exceptions;
using AuthMicroservice.Application.Interface;
using AuthMicroservice.Domain.Entities;
using AuthMicroservice.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthMicroservice.Infrastructure.Service
{
    public class PermissionService : IPermissionService
    {
        AppDbContext db;
        IMapper mapper;

        public PermissionService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<PermissionResponseDto> CreatePermission(CreatePermissionDto dto)
        {
            string name = dto.PermissionName;
            if (await db.Permissions.AnyAsync(p => p.PermissionName == name))
            {
                return new PermissionResponseDto { Message = "Permission already exists" };
            }
                
            var permission = mapper.Map<Permission>(dto);
            permission.PermissionName = name;
            await db.Permissions.AddAsync(permission);
            await db.SaveChangesAsync();
            return mapper.Map<PermissionResponseDto>(permission);
        }

    }
}
