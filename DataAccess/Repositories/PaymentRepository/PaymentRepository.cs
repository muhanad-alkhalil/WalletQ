using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletQ.Models;

namespace WalletQ.DataAccess.Repositories.PaymentRepository
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<Payment>> GetAllPayment(int page, Guid id)
        {
            return await _context.Payments
                .Where(p => p.creator.Id == id)
                .OrderByDescending(p => p.CreatedAt)
                .Include(p => p.transaction)
                .Skip(10 * (page-1))
                .Take(10)
                .ToListAsync();
        }

        public async Task<Payment> GetPayment(Guid id)
        {
            return await _context.Payments.Where(x => x.Id == id)
                .Include(x => x.creator)
                .Include(x => x.creator.wallet)
                .Include(x => x.transaction)
                .FirstOrDefaultAsync();
        }

        public async Task<int> PaymentsCount(Guid id)
        {
            return await _context.Payments
                .Where(p => p.creator.Id == id)
                .CountAsync();
        }
    }
}
