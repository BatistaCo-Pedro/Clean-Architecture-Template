namespace Infrastructure.Implementations;

/// <summary>
/// Masstransit implementation of the domain event dispatcher.
/// </summary>
/// <param name="mediator">The <see cref="MassTransit"/> <see cref="IScopedMediator"/>.</param>
/// <remarks>
/// To use this dispatcher, you must have a mass transit configured
/// and a scoped mediator added to the dependency container.
/// </remarks>
public class MassTransitDomainEventDispatcher(IScopedMediator mediator) : IDomainEventDispatcher
{
    /// <inheritdoc />
    public async Task DispatchAndClear(IEnumerable<Entity> entitiesWithEvents)
    {
        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();

            foreach (var domainEvent in events)
            {
                await mediator.Send(domainEvent).ConfigureAwait(false);
            }
        }
    }
}