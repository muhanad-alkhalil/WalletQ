using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using WalletQ.Models;
using WalletQ.DataAccess.Repositories;

namespace WalletQ.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly Repository<Payment> _repository;

        public PaymentService(Repository<Payment> repository)
        {
            _repository = repository;
        }
        public async Task<bool> Create(Payment payment)
        {
            _repository.Add(payment);
            return await _repository.Save();
        }
        public async Task<bool> Cancel(Payment payment)
        {
            _repository.Delete(payment);
            return await _repository.Save();
        }

       

        public Task<bool> Pay(Payment payment, User payer)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Payment payment)
        {
            throw new NotImplementedException();
        }
    }
}
