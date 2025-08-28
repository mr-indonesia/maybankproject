using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Models.Users;

namespace App.Core.Interfaces
{
    // Dependency Inversion Principle (DIP): Abstracts authentication handling.
    public interface IAuthenticationHandler
    {
        void Authenticate(User user);
    }
}
