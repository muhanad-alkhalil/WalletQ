using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletQ.DTOs.TransactionDTO
{
    public class TransactionPlainDTO
    {
        public string Type { get; set; }
        public string With { get; set; }
        public uint Amount { get; set; }
    }
}
