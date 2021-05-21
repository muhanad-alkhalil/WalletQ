using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletQ.Models;

namespace WalletQ.DataAccess.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {

        public UserRepository(DataContext context) : base(context) {}

        public async Task<User> getUser(Guid id)
        {
            return await _context.Users.Where(x => x.Id == id).Include(x => x.wallet).FirstOrDefaultAsync();
        }

        public async Task<User> getUserByWalletId(Guid id)
        {
            return await _context.Users.Where(x => x.wallet.Id == id).Include(x => x.wallet).FirstOrDefaultAsync();
        }

        public async Task<bool> isEmailExists(string name)
        {
            if (await _context.Users.AnyAsync(x => x.Email == name))
                return true;
            return false;
        }
    }
}
