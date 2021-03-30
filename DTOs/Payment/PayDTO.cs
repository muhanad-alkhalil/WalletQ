using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletQ.DTOs.Payment
{
    public class PayDTO
    {
        public Guid paymentId { get; set; }
        public string Password { get; set; }
    }
}
