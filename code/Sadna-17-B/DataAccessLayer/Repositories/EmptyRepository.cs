using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.Repositories;
using Sadna_17_B.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Sadna_17_B.DataAccessLayer.Repositories
{
    public class EmptyRepository<TEntity> : IRepository<TEntity> where TEntity : class

    {
        public EmptyRepository()
        {
        }

        public void Add(TEntity entity)
        {
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return null;
        }

        public TEntity Get(int id)
        {
            return null;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return null;
        }

        public void Remove(TEntity entity)
        {
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
        }

        public void DeleteAll()
        {
        }

        public void ResetAutoIncrementKey()
        {
        }
    }
}