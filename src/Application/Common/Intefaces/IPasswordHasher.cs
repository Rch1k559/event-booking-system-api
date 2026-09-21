using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Intefaces
{
    public interface IPasswordHasher
    {
        bool VerifyPassword(string password, string passwordHash);
        public string HashPassword(string password);
    }
}
