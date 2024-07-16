using Sadna_17_B.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Sadna_17_B.Repositories.Implementations
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(ApplicationDbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public TEntity Get(int id)
        {
            return DbSet.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return DbSet.ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public void Add(TEntity entity)
        {
            DbSet.Add(entity);
            Context.SaveChanges();
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
            Context.SaveChanges();
        }

        public void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
            Context.SaveChanges();
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
            Context.SaveChanges();
        }

        /*public void Update(TEntity entity)
        {
            DbSet.Remove(entity);
            Context.SaveChanges();
            DbSet.Add(entity);
            Context.SaveChanges();
        }*/

        // Implementing the Update method differently
        public void Update(TEntity entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public void DeleteAll()
        {
            var allEntities = DbSet.ToList();
            RemoveRange(allEntities);
            ResetAutoIncrementKey();
            Context.SaveChanges(); 
        }

        public void ResetAutoIncrementKey()
        {
            // Choose the appropriate implementation based on your database
            var tableName = Context.GetTableName(typeof(TEntity));
            Context.Database.ExecuteSqlCommand($"DBCC CHECKIDENT('{tableName}', RESEED, 0)");
            Context.SaveChanges();
        }
    }
}