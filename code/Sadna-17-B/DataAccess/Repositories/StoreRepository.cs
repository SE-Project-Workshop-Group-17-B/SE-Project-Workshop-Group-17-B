// DomainLayer/Repositories/StoreRepository.cs
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Sadna_17_B.Data;
using Sadna_17_B.DomainLayer.Entities;

namespace Sadna_17_B.DomainLayer.Repositories
{
    public class StoreRepository : GenericRepository<Store>, IStoreRepository
    {
        public StoreRepository(TradingSystemContext context) : base(context) { }

        public IEnumerable<Store> GetByName(string name)
        {
            return _dbSet.Where(s => s.Name.Contains(name)).ToList();
        }
    }
}