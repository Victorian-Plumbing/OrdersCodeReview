using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

//Not super familliar with EF Core (from an ADO.NET background), but this looks okay, apart from being beholden to I/O bound operations
public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _dbSet = context.Set<T>();
    }

    //May want to consider pagination (offset, fetch first x records). In addition, depending on the dataset,
    //might want to have default sort conditions (based on index makeup)
    public IQueryable<T> Get(Expression<Func<T, bool>>? filter)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
            query = query.Where(filter);

        return query;
    }

    public T Insert(T entity)
    {
        return _dbSet.Add(entity).Entity;
    }
}