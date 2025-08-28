using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Interfaces;
using App.Core.Models.Users;

namespace App.Core.Services
{
    public class UserService : IUserService
    {
        public User GetUserByUsername(string username)
        {
            if(username == "admin")
                return new User { Username = "admin", HashedPassword = "hashedPassword123" };

            return null;
        }
    }
}
