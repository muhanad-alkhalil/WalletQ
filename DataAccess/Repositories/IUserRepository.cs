using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletQ.Models;

namespace WalletQ.DataAccess.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<bool> isEmailExists(string name);

    }
}
