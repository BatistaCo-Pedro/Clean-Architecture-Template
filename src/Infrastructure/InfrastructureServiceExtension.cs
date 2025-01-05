namespace Infrastructure;

/// <summary>
/// Extension class containing dependency injection methods.
/// </summary>
public static class InfrastructureServiceExtension
{
    // Add options here from the config file if they need to be in the container
    // public static IServiceCollection AddValidatedOptions<TOptions>()

    /// <summary>
    /// Adds the database to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The connection string the database connects to.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        string connectionString
    )
    {
        services.AddScoped<IDomainEventDispatcher, MassTransitDomainEventDispatcher>();

        services.AddDbContext<SomeContext>(options =>
            options
                .UseNpgsql(
                    connectionString,
                    optionsBuilder =>
                    {
                        optionsBuilder
                            .EnableRetryOnFailure()
                            .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    }
                )
        );

        services.AddScoped<IUnitOfWork, SomeContext>();

        return services;
    }
}
