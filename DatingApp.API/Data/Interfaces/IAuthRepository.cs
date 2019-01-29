using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data.Interfaces
{
    public interface IAuthRepository
    {
         Task<User> Register(User user);
         Task<User> GetByUsername(string username);
    }
}