namespace Application.Models.Events;

/// <summary>
/// Base class for domain events.
/// </summary>
public abstract record DomainEventBase : IDomainEvent
{
    /// <summary>
    /// The date the event occurred.
    /// </summary>
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}