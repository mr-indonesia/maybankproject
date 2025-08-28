using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Models;
using App.Core.Models.Authentications;
using App.Core.Models.Users;
using SharedKernel.Entities;
using SharedKernel.Interfaces;

namespace App.Core.Interfaces
{
    public interface IIdentityService : IRepository
    {
        Task<ResponseModel<TokenModel>> Login(string sessionid);
        Task<AuthenticationResult> AuthecticateAsync(UserModel user);
    }
}
