using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletQ.DTOs.User;
using WalletQ.Models;

namespace WalletQ.DataAccess.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(LoginDTO userData)
        {
            userData.Email = userData.Email.ToLower();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userData.Email);
            if (user == null)
                return null;
            if (!VerfyPasswordHash(userData.Password, user.PasswordHash, user.PasswordSalt))
                return null;
            return user;
        }
        private bool VerfyPasswordHash(string Password, byte[] PasswordHash, byte[] PassWordSalt)
        {
            using (var Hmac = new System.Security.Cryptography.HMACSHA512(PassWordSalt))
            {
                var computedHash = Hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != PasswordHash[i])
                        return false;
                }
                return true;
            }
        }
    }
}
