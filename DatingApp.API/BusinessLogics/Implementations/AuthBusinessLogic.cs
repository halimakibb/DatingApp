using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.BusinessLogics.Interfaces;
using DatingApp.API.Data.Interfaces;
using DatingApp.API.Models;
using DevOne.Security.Cryptography.BCrypt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.BusinessLogics.Implementations
{
    public class AuthBusinessLogic : IAuthBusinessLogic
    {
        private IAuthRepository _authRepository;
        private readonly IConfiguration _config;
        public AuthBusinessLogic(IAuthRepository authRepository, IConfiguration config)
        {
            _authRepository = authRepository;
            _config = config;
        }
        public async Task<bool> IsUserExists(string username)
        {
            bool isUserExists = true;
            try
            {
                User currentUser = await _authRepository.GetByUsername(username);
                if (currentUser == null)
                {
                    isUserExists = false;
                }
                return isUserExists;
            }
            catch (Exception ex)
            {
                return isUserExists;
            }
        }

        public async Task<string> Login(string username, string password)
        {
            try
            {
                User currentUser = await _authRepository.GetByUsername(username);

                if (currentUser != null)
                {
                    if (BCryptHelper.CheckPassword(password, currentUser.PasswordHash))
                    {
                        Claim[] claims = new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, currentUser.Id.ToString()),
                            new Claim(ClaimTypes.Name, currentUser.UserName)
                        };

                        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8
                            .GetBytes(_config.GetSection("AppSettings:Token").Value));

                        SigningCredentials cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(claims),
                            Expires = DateTime.Now.AddDays(1),
                            SigningCredentials = cred
                        };

                        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
                        return tokenHandler.WriteToken(token);
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                string x = ex.Message;
                return null;
            }
        }

        public async Task<User> Register(User user, string password)
        {
            try 
            {
                string passwordHash = CreatePasswordHash(password);
                user.PasswordHash = passwordHash;
                return await _authRepository.Register(user);
            } 
            catch (Exception ex) 
            {
                return null;
            }
        }

        #region Private Methods
        private string CreatePasswordHash(string password)
        {
            return BCryptHelper.HashPassword(password, BCryptHelper.GenerateSalt(12));
        }
        #endregion
    }
}