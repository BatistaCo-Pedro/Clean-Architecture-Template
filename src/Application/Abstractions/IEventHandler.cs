namespace Application.Abstractions;

/// <summary>
/// Interface for handlers.
/// </summary>
/// <typeparam name="TEventMessage">The type of the event.</typeparam>
/// <typeparam name="TResult">The type of the output.</typeparam>
/// <remarks>
/// Event handlers aren't the first point of contact for events, they get injected into a consumer which then calls the <see cref="Handle"/> method.
/// </remarks>
public interface IEventHandler<in TEventMessage, TResult>
    where TEventMessage : class, IEventMessage
    where TResult : class
{
    /// <summary>
    /// Handles an event of type <see cref="TEventMessage"/>.
    /// </summary>
    /// <param name="eventMessage">The event message with containing information about the event.</param>
    /// <returns>A <see cref="Task"/> containign the Result of type <see cref="TResult"/>.</returns>
    public Task<TResult> Handle(TEventMessage eventMessage);
}

/// <summary>
/// Interface for handlers.
/// </summary>
/// <typeparam name="TEventMessage">The type of the event</typeparam>
/// <remarks>
/// Event handlers aren't the first point of contact for events, they get injected into a consumer which then calls the <see cref="Handle"/> method.
/// </remarks>
public interface IEventHandler<in TEventMessage>
    where TEventMessage : class, IEventMessage
{
    /// <summary>
    /// Handles an event of type <see cref="TEventMessage"/>.
    /// </summary>
    /// <param name="eventMessage">The event message with containing information about the event.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    public Task Handle(TEventMessage eventMessage);
}
