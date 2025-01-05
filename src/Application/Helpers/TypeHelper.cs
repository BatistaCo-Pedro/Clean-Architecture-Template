namespace Application.Helpers;

/// <summary>
/// Helper class for registering types.
/// </summary>
public static class TypeHelper
{
    /// <summary>
    /// Gets the event handler types mapped to their event types.
    /// </summary>
    /// <param name="typesToSearchThrough">The types to search through to find the event handlers.</param>
    /// <returns>A <see cref="Dictionary{TKey,TValue}"/> with the event handler type as value and the  as value.</returns>
    public static Dictionary<Type, Type> GetEventHandlerTypes(Type[] typesToSearchThrough) =>
        typesToSearchThrough
            .Where(x =>
                x.GetInterfaces()
                    .Any(y =>
                        y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IEventHandler<>)
                    )
            )
            .ToDictionary(x =>
                x.GetInterfaces()
                    .First(y =>
                        y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IEventHandler<>)
                    )
                    .GetGenericArguments()
                    .First()
            );

    /// <summary>
    /// Checks whether the given type is a domain event consumer.
    /// </summary>
    public static readonly Predicate<Type> IsDomainEvent = consumerType =>
        consumerType.GetInterfaces().Contains(typeof(IDomainEvent));
}
