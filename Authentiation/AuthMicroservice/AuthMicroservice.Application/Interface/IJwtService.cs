using AuthMicroservice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthMicroservice.Application.Interface
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
