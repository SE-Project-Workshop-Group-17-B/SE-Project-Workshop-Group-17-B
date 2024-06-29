using System.Collections.Generic;
using System.Linq.Expressions;
using System;

public interface IGenericRepository<T> where T : class
{
    T GetById(int id);
    IEnumerable<T> GetAll();
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
}