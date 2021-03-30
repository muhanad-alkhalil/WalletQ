using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletQ.Models;

namespace WalletQ.Services.PaymentService
{
    interface IPaymentService
    {
        public Task<bool> Create(Payment payment);
        public Task<bool> Cancel(Payment payment);
        public Task<bool> Update(Payment payment);
        public Task<bool> Pay(Payment payment,User payer);
    }
}
