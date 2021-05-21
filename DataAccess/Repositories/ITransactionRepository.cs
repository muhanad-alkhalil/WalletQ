using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletQ.Models;

namespace WalletQ.DataAccess.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        public Task<IEnumerable<Transaction>> GetLastTransactions(Guid id);
    }
}
