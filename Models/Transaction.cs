using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletQ.Models
{
    public class Transaction : BaseEntity
    {
        public uint Amount { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReciverId { get; set; }
    }
}
