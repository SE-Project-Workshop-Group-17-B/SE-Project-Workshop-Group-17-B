// DomainLayer/Repositories/IStoreRepository.cs
using Sadna_17_B.Data;
using Sadna_17_B.DomainLayer.Entities;
using System.Data.Entity;

namespace Sadna_17_B.DomainLayer.Repositories
{
    public interface IStoreRepository
    {
        Store GetById(int id);
        void Add(Store store);
        void Update(Store store);
        void Delete(int id);
    }
}

