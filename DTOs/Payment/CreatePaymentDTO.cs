using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletQ.DTOs.Payment
{
    public class CreatePaymentDTO
    {
        public uint amount { get; set; }
        public int validationTimeByMinutes { get; set; }
    }
    
}
