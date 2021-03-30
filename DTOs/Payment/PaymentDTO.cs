using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletQ.DTOs.User;
using WalletQ.Models;

namespace WalletQ.DTOs.Payment
{
    public class PaymentDTO
    {
        public PaymentUserDTO creator { get; set; }
        public Guid WalletId { get; set; }
        public PaymentState paymentState { get; set; }
        public Transaction transaction { get; set; }
        public DateTime validUntill { get; set; }
        public uint amount { get; set; }
    }
}
