namespace Application.Abstractions;

/// <summary>
/// Represents a repository with write operations.
/// </summary>
/// <typeparam name="TEntity">The type this repository acts for. Must be an <see cref="Entity"/>.</typeparam>
public interface IWriteRepository<TEntity> : IRepository<TEntity>
    where TEntity : AggregateRoot
{
    /// <summary>
    /// Creates an entity.
    /// </summary>
    /// <param name="dataOwners">The entities to create.</param>
    /// <returns>A boolean defining if the entity was created.</returns>
    /// <remarks>This method does not call <see cref="IUnitOfWork.SaveChanges()"/>, it only adds the entity to the ChangeTracker.</remarks>
    void Create(params TEntity[] dataOwners);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entities">The entities to delete</param>
    /// <returns>A boolean defining if the entity was deleted.</returns>
    /// <remarks>This method does not call <see cref="IUnitOfWork.SaveChanges()"/>, it only adds the entity to the ChangeTracker.</remarks>
    void Delete(params TEntity[] entities);
}
