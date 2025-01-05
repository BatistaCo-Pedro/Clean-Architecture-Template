namespace Application.Abstractions;

/// <summary>
/// Represents a repository that supports CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The type of the entity for the repo.</typeparam>
public interface ICrudRepository<TEntity> : IReadRepository<TEntity>, IWriteRepository<TEntity>
    where TEntity : AggregateRoot;
