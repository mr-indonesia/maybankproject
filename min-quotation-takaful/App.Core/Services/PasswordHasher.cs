using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Interfaces;

namespace App.Core.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            // Simulate password hashing logic
            return plainPassword == "password123" && hashedPassword == "hashedPassword123";
        }
    }
}
