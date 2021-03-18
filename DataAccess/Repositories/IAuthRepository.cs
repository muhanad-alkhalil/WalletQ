using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletQ.DTOs.User;
using WalletQ.Models;

namespace WalletQ.DataAccess.Repositories
{
    public interface IAuthRepository
    {
        Task<User> Login(LoginDTO userData);
    }
}
