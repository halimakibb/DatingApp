using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.BusinessLogics.Interfaces
{
    public interface IAuthBusinessLogic
    {
         Task<User> Register(User user, string password);
         Task<string> Login(string username, string password);
         Task<bool> IsUserExists(string username);
    }
}