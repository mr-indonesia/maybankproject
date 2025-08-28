using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using App.Core.Interfaces;
using App.Core.Models;
using App.Core.Models.Authentications;
using App.Core.Models.Roles;
using App.Core.Models.Users;
using DataAccess.EFCore;
using DataAccess.EFCore.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Entities;
using SharedKernel.Interfaces;

namespace App.Core.Services
{
    public class IdentityService : EFRepository,IIdentityService
    {
        private readonly IRepository repository;
        private readonly ServiceConfiguration _serviceConfiguration;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public IdentityService(ApplicationContext context, IRepository _repository, IOptions<ServiceConfiguration> serviceConfiguration, TokenValidationParameters tokenValidationParameters) : base(context)
        {
            this.repository = _repository;
            _serviceConfiguration = serviceConfiguration.Value;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<AuthenticationResult> AuthecticateAsync(UserModel user)
        {
            //define authetication result
            AuthenticationResult result = new AuthenticationResult();

            //define token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_serviceConfiguration.JwtSettings.Secret));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

                //define claim identity
                ClaimsIdentity subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Username", user.Username),
                    new Claim("FullName", user.FullName),
                    new Claim("RoleCode", user.RoleCode),
                    new Claim("Email", user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                });

                //define role from get roles
                foreach (var item in await GetRoles(user))
                {
                    subject.AddClaim(new Claim(ClaimTypes.Role, item.RoleCode));
                }

                //define token descriptor
                var tokenDescriptor = new SecurityTokenDescriptor { 
                    Subject = subject,
                    Expires = DateTime.UtcNow.Add(_serviceConfiguration.JwtSettings.TokenLifeTime),
                    SigningCredentials = credentials
                    //SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                };

                //create token
                /*var token = new JwtSecurityToken(_serviceConfiguration.JwtSettings.Issuer,
                                                 _serviceConfiguration.JwtSettings.Issuer,
                                                 null,
                                                 expires: DateTime.UtcNow.Add(_serviceConfiguration.JwtSettings.TokenLifeTime),
                                                 signingCredentials: credentials);*/

                var token = tokenHandler.CreateToken(tokenDescriptor);
                result.Token = tokenHandler.WriteToken(token);

                //refresh token
                var refreshToken = new RefreshToken { 
                    Token = Guid.NewGuid().ToString(),
                    JwtId = token.Id,
                    UserId = user.UserId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    ExpireDate = DateTime.UtcNow.AddMinutes(10)
                };

                //save refresh token to db
                /*string query = @"INSERT INTO RefreshToken(RefreshTokenId,Token,JwtId,UserId,CreatedAt,CreatedBy,ExpireDate)
                                 VALUES(@RefreshTokenId,@Token,@JwtId,@UserId,@CreatedAt,@CreatedBy,@ExpireDate)";
                var param = new Dictionary<string, object>
                {
                    {"@RefreshTokenId", Guid.NewGuid().ToString() },
                    {"@Token", refreshToken.Token },
                    {"@JwtId", refreshToken.JwtId },
                    {"@UserId", refreshToken.UserId },
                    {"@CreatedAt", refreshToken.CreatedAt },
                    {"@CreatedBy", refreshToken.CreatedBy },
                    {"@ExpireDate", refreshToken.ExpireDate }
                };

                await repository.ExecuteQueryAsync(query, param, false);*/
                //await context.SaveChangesAsync();

                //set token result
                result.RefreshToken = refreshToken.Token;
                result.Success = true;

                return result;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async Task<ResponseModel<TokenModel>> Login(string sessionid)
        {
            ResponseModel<TokenModel> response = new ResponseModel<TokenModel>();

            try
            {
                string query = @"SELECT
                                Username = a.CODE,
                                FullName = UPPER(ISNULL(a.FRONT_NAME,'') + REPLACE(' ' + ISNULL(MID_NAME,'') + ' ','  ',' ') + ISNULL(a.LAST_NAME,'')),
                                RoleCode = a.ROLE_CODE,
                                Email = a.EMAIL
                                FROM SECURITY.dbo.M_USERS a
                                INNER JOIN SECURITY.dbo.USER_LOG_HISTORY b on a.CODE=b.USER_CODE
                                WHERE
                                b.ROWID = @SessionId
                                AND isnull(a.ACTIVE,0) > 0
                                --AND a.CODE = 'johan'
                                AND b.LOGOUT IS NULL";
                var param = new Dictionary<string, object>
                {
                    {"@SessionId", sessionid }
                };

                //conn.QueryString = query;
                var data = await repository.QueryAsync<UserModel>(query, param, false);
                var user = data.FirstOrDefault();

                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Username not found";

                    return response;
                }

                //define authenticate token
                AuthenticationResult authenticationResult = await AuthecticateAsync(user);
                if (authenticationResult != null && authenticationResult.Success) {
                    response.Data = new TokenModel { 
                        Token = authenticationResult.Token,
                        RefreshToken = authenticationResult.RefreshToken
                    };
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Something went wrong";
                }

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<List<RoleModel>> GetRoles(UserModel user) {
            try
            {
                //get role by userid
                string query = @"SELECT RoleCode = APP_CODE, RoleName = APP_NAME 
                                FROM  SECURITY.dbo.V_ROLE_APPS 
                                WHERE ROLE_CODE = @RoleCode
                                ORDER BY APP_NAME";
                var param = new Dictionary<string, object>
                {
                    {"@RoleCode", user.RoleCode }
                };
                var data = await repository.QueryAsync<RoleModel>(query, param, false);

                return data;
            }
            catch (Exception)
            {

                return new List<RoleModel>();
            }
        }
    }
}
