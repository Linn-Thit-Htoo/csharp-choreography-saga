using System.Linq.Expressions;
using csharp_choreography_saga.StockMicroservice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace csharp_choreography_saga.StockMicroservice.Persistence.Base;

public class RepositoryBase<T> : IRepositoryBase<T>
    where T : class
{
    internal readonly DbContext _context;
    internal readonly DbSet<T> _dbSet;

    public RepositoryBase(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> GetAll()
    {
        return _dbSet.AsQueryable();
    }

    public IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression)
    {
        return _dbSet.Where(expression);
    }

    public IQueryable<T> Query(
        Expression<Func<T, bool>> expression = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> order = null
    )
    {
        IQueryable<T> query = _dbSet;

        if (include is not null)
        {
            query = include(query);
        }
        if (expression is not null)
        {
            query = query.Where(expression);
        }
        if (order is not null)
        {
            query = order(query);
        }

        return query;
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void Add(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    public async Task AddAsync(T entity, CancellationToken cs = default)
    {
        await _dbSet.AddAsync(entity, cs);
    }

    public async Task AddAsync(IEnumerable<T> entities, CancellationToken cs = default)
    {
        await _dbSet.AddRangeAsync(entities, cs);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Update(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void Delete(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    public async Task SaveChangesAsync(CancellationToken cs)
    {
        await _context.SaveChangesAsync(cs);
    }
}
