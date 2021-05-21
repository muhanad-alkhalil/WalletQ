using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletQ.Models;

namespace WalletQ.DataAccess.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<Transaction>> GetLastTransactions(Guid id)
        {
            return await _context.Transactions
                .Where(Transaction => Transaction.Reciver.Id == id || Transaction.Sender.Id == id)
                .OrderByDescending(T => T.CreatedAt)
                .Take(5)
                .Include(T => T.Reciver)
                .Include(T => T.Sender)
                .ToListAsync();
        }

    }
}
