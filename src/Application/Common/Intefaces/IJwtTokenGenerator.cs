using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Intefaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
