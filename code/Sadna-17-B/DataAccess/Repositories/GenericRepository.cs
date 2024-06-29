using Sadna_17_B.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using System;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly TradingSystemContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(TradingSystemContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual T GetById(int id)
    {
        return _dbSet.Find(id);
    }

    public virtual IEnumerable<T> GetAll()
    {
        return _dbSet.ToList();
    }

    public virtual void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public virtual void Delete(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);
    }

    public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate);
    }
}