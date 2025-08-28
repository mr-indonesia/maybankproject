using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Interfaces;
using App.Core.Models.Users;

namespace App.Core.Services
{
    public class AuthenticationHandlerService : IAuthenticationHandler
    {
        public void Authenticate(User user)
        {
            // Simulated authentication logic (e.g., generating a session token)
            Console.WriteLine($"User '{user.Username}' authenticated successfully.");
        }
    }
}
