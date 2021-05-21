using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletQ.DTOs.TransactionDTO
{
    public class CreateTransactionDTO
    {
        public string id { get; set; }
        public uint amount { get; set; }
    }
}
