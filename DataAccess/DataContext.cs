using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletQ.Models;

namespace WalletQ.DataAccess
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}
            public DbSet<Wallet> Wallets { get; set; } 
            public DbSet<User> Users { get; set; }
            public DbSet<Transaction> Transactions { get; set; }


    }
    
}
