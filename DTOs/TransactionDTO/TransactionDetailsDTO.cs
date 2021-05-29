using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletQ.DTOs.TransactionDTO
{
    public class TransactionDetailsDTO
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Type { get; set; }
        public string With { get; set; }
        public uint Amount { get; set; }
    }
}
