namespace Infrastructure.Events;

/// <summary>
/// Interface for domain event dispatchers.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches and clears all domain events from the given entities.
    /// </summary>
    /// <param name="entitiesWithEvents">The entities to dispatch and clear events for.</param>
    public Task DispatchAndClear(IEnumerable<Entity> entitiesWithEvents);
}