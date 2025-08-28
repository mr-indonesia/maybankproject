using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Interfaces
{
    // Dependency Inversion Principle (DIP): Interface for password hashing logic.
    public interface IPasswordHasher
    {
        bool VerifyPassword(string plainPassword, string hashedPassword);
    }
}
