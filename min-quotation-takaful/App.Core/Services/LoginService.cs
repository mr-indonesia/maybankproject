using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Interfaces;

namespace App.Core.Services
{
    // Single Responsibility Principle (SRP): Encapsulates login logic.
    public class LoginService : ILoginService
    {
        #region Private Variable
        static string sharedSecret = "01012015";// System.Configuration.ConfigurationManager.AppSettings["enckey"];
        static IEncryptionService encryptionService = EncryptionServiceExt.CreateEncryptionService(sharedSecret);
        static string stringEnc = "strencript";
        static string connString = encryptionService.Decrypt(stringEnc);
        #endregion
        private readonly IUserService _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger _logger;
        private readonly IAuthenticationHandler _authenticationHandler;
        public LoginService(IUserService userService, IPasswordHasher passwordHasher, ILogger logger, IAuthenticationHandler authenticationHandler) { 
            _userRepository = userService;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _authenticationHandler = authenticationHandler;
        }

        public bool Login(string username, string password)
        {
            try
            {
                // Retrieve user from repository
                var user = _userRepository.GetUserByUsername(username);
                if (user == null)
                {
                    _logger.LogWarning($"Login failed: User '{username}' not found.");
                    return false;
                }

                // Verify password
                if (!_passwordHasher.VerifyPassword(password, user.HashedPassword))
                {
                    _logger.LogWarning($"Login failed: Incorrect password for user '{username}'.");
                    return false;
                }

                // Authenticate the user (e.g., create a session or token)
                _authenticationHandler.Authenticate(user);

                _logger.LogInfo($"User '{username}' successfully logged in.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error for user '{username}': {ex.Message}");
                return false;
            }
        }
    }
}
