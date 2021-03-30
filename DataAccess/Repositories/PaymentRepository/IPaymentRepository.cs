using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletQ.Models;

namespace WalletQ.DataAccess.Repositories.PaymentRepository
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment> GetPayment(Guid id);
    }
}
