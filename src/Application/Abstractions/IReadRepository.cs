namespace Application.Abstractions;

/// <summary>
/// Represents a repository with read operations.
/// </summary>
/// <typeparam name="TEntity">The type this repository acts for. Must be an <see cref="Entity"/>.</typeparam>
public interface IReadRepository<TEntity> : IRepository<TEntity>
    where TEntity : AggregateRoot
{
    /// <summary>
    /// Gets all entities from the table.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of type <see cref="TEntity"/> containing all entities from the table.</returns>
    IEnumerable<TEntity> GetAll();

    /// <summary>
    /// Gets entities by a condition.
    /// </summary>
    /// <param name="condition">The condition to get entities by.</param>
    /// <param name="orderBy">The condition to order by.</param>
    /// <param name="includeProperties">The properties to include.</param>
    /// <returns>A <see cref="IQueryable"/> with entities matching the condition parameter.</returns>
    IEnumerable<TEntity> GetByCondition(
        Expression<Func<TEntity, bool>> condition,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = ""
    );

    /// <summary>
    /// Gets an entity by id.
    /// </summary>
    /// <param name="id">The id from the entity.</param>
    /// <returns>A <see cref="Result"/> with the entity which matched the id or an error.</returns>
    Result<TEntity> GetById(Guid id);
}
