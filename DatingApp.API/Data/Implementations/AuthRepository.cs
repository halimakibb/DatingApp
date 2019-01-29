using System;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data.Interfaces;
using DatingApp.API.Models;
using DevOne.Security.Cryptography.BCrypt;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
       
        public async Task<User> GetByUsername(string username)
        {
            try
            {
               return await _context.Users.FirstOrDefaultAsync(user => user.UserName == username);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> Register(User user)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();
                    dbContextTransaction.Commit();
                    return user;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }
            
        }
    }
}