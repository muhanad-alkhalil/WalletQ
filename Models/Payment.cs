using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletQ.Models
{
    public class Payment : BaseEntity
    {
        public User creator { get; set; }
        public PaymentState paymentState { get; set; }
        public Transaction transaction { get; set; }
        public int ValidationTime { get; set; }

        public Payment()
        { 
            paymentState = PaymentState.Pending;
        }
    }

    public enum PaymentState
    {
        Pending,
        Paid,
        Expired,
        Cancelled
    }
}

