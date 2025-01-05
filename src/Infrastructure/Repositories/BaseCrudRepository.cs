namespace Infrastructure.Repositories;

/// <summary>
/// Base class for a crud repository.
/// </summary>
/// <param name="db">The context of the database.</param>
/// <typeparam name="TEntity">The type of entity this repository acts for.</typeparam>
internal class BaseCrudRepository<TEntity>(SomeContext db) : ICrudRepository<TEntity>
    where TEntity : AggregateRoot
{
    /// <inheritdoc />
    public virtual IEnumerable<TEntity> GetAll()
    {
        return db.Set<TEntity>().AsEnumerable();
    }

    /// <inheritdoc />
    public virtual IEnumerable<TEntity> GetByCondition(
        Expression<Func<TEntity, bool>> condition,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = ""
    )
    {
        var query = db.Set<TEntity>().Where(condition);

        foreach (
            var includeProperty in includeProperties.Split(
                new[] { ',', ';', '_' },
                StringSplitOptions.RemoveEmptyEntries
            )
        )
        {
            query = query.Include(includeProperty);
        }

        return orderBy != null ? orderBy(query).ToList() : query.ToList();
    }

    /// <inheritdoc />
    public virtual Result<TEntity> GetById(Guid id) =>
        db.Set<TEntity>()
            .Find(id)
            .ToResult($"Entity {typeof(TEntity).Name} with ID: `{id}` not found.");

    /// <inheritdoc />
    public virtual void Create(params TEntity[] entities) => db.Set<TEntity>().AddRange(entities);

    /// <inheritdoc />
    public virtual void Delete(params TEntity[] entities) =>
        db.Set<TEntity>().RemoveRange(entities);
}
