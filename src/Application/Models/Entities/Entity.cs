namespace Application.Models.Entities;

/// <summary>
/// Abstract entity type to be inherited by database entities.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// The domain events that have been raised.
    /// </summary>
    private readonly HashSet<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Gets the domain events.
    /// </summary>
    [NotMapped]
    public IReadOnlySet<IDomainEvent> DomainEvents => _domainEvents;

    /// <summary>
    /// The identifier of the entity. ULID as GUID.
    /// </summary>
    /// <remarks>
    /// ULID spec: https://github.com/ulid/spec
    /// </remarks>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; protected set; } = Ulid.NewUlid().ToGuid();

    /// <summary>
    /// Raise a domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to raise.</param>
    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Clears all domain events from the entity.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
