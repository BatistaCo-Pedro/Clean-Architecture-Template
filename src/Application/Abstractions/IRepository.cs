namespace Application.Abstractions;

/// <summary>
/// Represents a repository.
/// </summary>
public interface IRepository;

/// <summary>
/// Generic repository interface.
/// </summary>
/// <typeparam name="TEntity">The type of the entity. Must be <see cref="AggregateRoot"/>.</typeparam>
public interface IRepository<TEntity> : IRepository
    where TEntity : AggregateRoot;
