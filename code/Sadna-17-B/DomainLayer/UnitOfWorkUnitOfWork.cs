// DomainLayer/UnitOfWork.cs
using Sadna_17_B.Data;
using Sadna_17_B.DomainLayer.Repositories;
using System;

namespace Sadna_17_B.DomainLayer
{
    public class UnitOfWork : IDisposable
    {
        private readonly TradingSystemContext _context;
        private IStoreRepository _storeRepository;

        public UnitOfWork()
        {
            _context = new TradingSystemContext();
        }

        public IStoreRepository Stores
        {
            get
            {
                if (_storeRepository == null)
                {
                    _storeRepository = new StoreRepository(_context);
                }
                return _storeRepository;
            }
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}