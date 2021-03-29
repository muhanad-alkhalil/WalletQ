using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletQ.Models;

namespace WalletQ.Services.PaymentService
{
    interface IPaymentService
    {
        public bool Create(Payment payment);
        public bool Cancel(Payment payment);
        public bool Update(Payment payment);
        public bool Pay(Payment payment,User payer);
    }
}
