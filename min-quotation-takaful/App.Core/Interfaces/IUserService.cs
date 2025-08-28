using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Models.Users;

namespace App.Core.Interfaces
{
    // Open/Closed Principle (OCP): Interface for user data access, allowing easy extension.
    public interface IUserService
    {
        User GetUserByUsername(string username);
    }
}
