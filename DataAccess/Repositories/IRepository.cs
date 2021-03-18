using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletQ.Models;

namespace WalletQ.DataAccess.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        Task<T> Get(Guid id);
        Task<IEnumerable<T>> GetAll();
        Task<bool> Save();

    }
}
