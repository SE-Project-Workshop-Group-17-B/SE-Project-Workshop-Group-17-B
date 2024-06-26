// DomainLayer/Repositories/StoreRepository.cs
using System.Data.Entity;
using Sadna_17_B.Data;
using Sadna_17_B.DomainLayer.Entities;

namespace Sadna_17_B.DomainLayer.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly TradingSystemContext _context;

        public StoreRepository(TradingSystemContext context)
        {
            _context = context;
        }

        public Store GetById(int id)
        {
            return _context.Stores.Find(id);
        }

        public void Add(Store store)
        {
            _context.Stores.Add(store);
        }

        public void Update(Store store)
        {
            _context.Entry(store).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var store = _context.Stores.Find(id);
            if (store != null)
            {
                _context.Stores.Remove(store);
            }
        }
    }
}