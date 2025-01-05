namespace Infrastructure.Persistence;

// Learn more about disposal pattern: https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
/// <summary>
/// The EF Core database context for the notification service.
/// Implements <see cref="IUnitOfWork"/>.
/// </summary>
internal sealed class SomeContext : DbContext, IUnitOfWork
{
    private const string DefaultLoggingContext = "Transaction";

    private readonly ConcurrentDictionary<string, IRepository> _repositories = [];

    private readonly IDomainEventDispatcher _domainEventDispatcher;

    /// <summary>
    /// Ctor for the <see cref="SomeContext"/>.
    /// </summary>
    /// <param name="options">Options to create the database.</param>
    /// <param name="domainEventDispatcher">The domain event to use - must be registered in the dependency container.</param>
    public SomeContext(
        DbContextOptions<SomeContext> options,
        IDomainEventDispatcher domainEventDispatcher
    )
        : base(options)
    {
        Log.Debug("Creating {DbContext}", nameof(SomeContext));

        SavingChanges += OnSavingChanges;

        _domainEventDispatcher = domainEventDispatcher;

        // Register custom repositories
        RegisterRepository<ITemplateTypeRepository, TemplateType>(new TemplateTypeRepository(this));
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TemplateType>().HasIndex(x => x.Name).IsUnique();
        modelBuilder
            .Entity<EmailBodyContent>()
            .HasIndex(x => new { x.CultureCode, x.EmailTemplateId })
            .IsUnique();
        modelBuilder
            .Entity<EmailTemplate>()
            .HasIndex(x => x.Name)
            .IsUnique()
            .AreNullsDistinct(false);

        modelBuilder
            .Entity<TemplateType>()
            .HasMany(x => x.EmailTemplates)
            .WithOne(x => x.TemplateType)
            .HasForeignKey(x => x.TemplateTypeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }

    /// <inheritdoc />
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<NonEmptyString>().HaveConversion<NonEmptyStringConverter>();
        configurationBuilder.Properties<CultureCode>().HaveConversion<CultureCodeConverter>();
        configurationBuilder.Properties<HtmlString>().HaveConversion<HtmlStringConverter>();
    }

    /// <inheritdoc />
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var entities = ChangeTracker
            .Entries<Entity>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Any())
            .ToList();

        _domainEventDispatcher.DispatchAndClear(entities).Wait();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <inheritdoc />
    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default
    )
    {
        var entities = ChangeTracker
            .Entries<Entity>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Any())
            .AsEnumerable();

        await _domainEventDispatcher.DispatchAndClear(entities);

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <inheritdoc />
    public bool RegisterRepository<TRepo, TEntity>(TRepo repository)
        where TEntity : AggregateRoot
        where TRepo : IRepository<TEntity>
    {
        return _repositories.TryAdd(typeof(TRepo).Name, repository);
    }

    /// <inheritdoc />
    public TRepo GetRepository<TRepo>()
        where TRepo : IRepository
    {
        var result = _repositories.TryGetValue(typeof(TRepo).Name, out var repository);

        if (result)
        {
            return (TRepo)repository!;
        }

        var entityType = typeof(TRepo).GetGenericArguments().Single();
        return (TRepo)
            Activator.CreateInstance(
                typeof(BaseCrudRepository<>).MakeGenericType(entityType),
                this
            )!;
    }

    /// <inheritdoc />
    public Result UseTransaction(
        Action action,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return Database
            .CreateExecutionStrategy()
            .Execute(() =>
            {
                using var transaction = Database.BeginTransaction();

                try
                {
                    action();
                    transaction.Commit();
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Result
                        .Fail(ex.Message);
                }
            });
    }

    /// <inheritdoc />
    public Result<T> UseTransaction<T>(
        Func<T> action,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return Database
            .CreateExecutionStrategy()
            .Execute(() =>
            {
                using var transaction = Database.BeginTransaction();

                try
                {
                    var result = action();
                    transaction.Commit();
                    return Result<T>
                        .Ok(result);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Result<T>
                        .Fail(ex.Message);
                }
            });
    }

    /// <inheritdoc />
    public Result<T> UseTransaction<T>(
        Func<Result<T>> action,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return Database
            .CreateExecutionStrategy()
            .Execute(() =>
            {
                using var transaction = Database.BeginTransaction();

                var result = action();
                if (!result.IsSuccess)
                {
                    transaction.Rollback();
                    return result;
                }

                transaction.Commit();
                return result;
            });
    }

    /// <inheritdoc />
    public async Task<Result> UseTransactionAsync(
        Func<Task> action,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return await Database
            .CreateExecutionStrategy()
            .ExecuteAsync(async () =>
            {
                await using var transaction = await Database.BeginTransactionAsync(
                    cancellationToken
                );

                try
                {
                    await action();
                    await transaction.CommitAsync(cancellationToken);
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result
                        .Fail(ex.Message)
                       ;
                }
            });
    }

    /// <inheritdoc />
    public async Task<Result<T>> UseTransactionAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return await Database
            .CreateExecutionStrategy()
            .ExecuteAsync(async () =>
            {
                await using var transaction = await Database.BeginTransactionAsync(
                    cancellationToken
                );

                try
                {
                    var result = await action();
                    await transaction.CommitAsync(cancellationToken);
                    return Result<T>
                        .Ok(result)
                        ;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<T>
                        .Fail(ex.Message)
                       ;
                }
            });
    }

    /// <inheritdoc />
    public async Task<Result<T>> UseTransaction<T>(
        Func<Task<Result<T>>> action,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return await Database
            .CreateExecutionStrategy()
            .ExecuteAsync(async () =>
            {
                await using var transaction = await Database.BeginTransactionAsync();

                var result = await action();
                if (!result.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return result;
                }

                await transaction.CommitAsync();
                return result;
            });
    }

    private void OnSavingChanges(object? sender, SavingChangesEventArgs args)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is not Entity entity)
                continue;

            Log.Debug(
                "Entity {EntityName} with id {Id} is {State}",
                entity.GetType().Name,
                entity.Id,
                entry.State
            );

            if (entry.Entity is not AuditableEntity auditableEntity)
                continue;

            if (entry.State == EntityState.Added)
            {
                auditableEntity.CreatedAt = DateTime.UtcNow;
                auditableEntity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                auditableEntity.Stamp++;
                auditableEntity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
