namespace Application.Abstractions;

/// <summary>
/// Interface representing a unit of work.
/// </summary>
public interface IUnitOfWork
{
    private const string DefaultLoggingContext = "Transaction";

    /// <summary>
    /// Gets a repository.
    /// </summary>
    /// <typeparam name="TRepo">The type of the repo to get.</typeparam>
    /// <returns>The repository as <c>T</c>.</returns>
    /// <exception cref="ArgumentException">Repository not found.</exception>
    public TRepo GetRepository<TRepo>()
        where TRepo : IRepository;

    /// <summary>
    /// Registers a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TRepo">The type of the repo.</typeparam>
    /// <returns>A boolean defining the result of the registration.</returns>
    public bool RegisterRepository<TRepo, TEntity>(TRepo repository)
        where TEntity : AggregateRoot
        where TRepo : IRepository<TEntity>;

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>
    /// The number of state entries written to the database.
    /// </returns>
    /// <exception cref="DbUpdateException">
    /// An error is encountered while saving to the database.
    /// </exception>
    /// <exception cref="DbUpdateConcurrencyException">
    /// A concurrency violation is encountered while saving to the database.
    /// A concurrency violation occurs when an unexpected number of rows are affected during save.
    /// This is usually because the data in the database has been modified since it was loaded into memory.
    /// </exception>
    int SaveChanges();

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous save operation. The task result contains the
    /// number of state entries written to the database.
    /// </returns>
    /// <exception cref="DbUpdateException">
    /// An error is encountered while saving to the database.
    /// </exception>
    /// <exception cref="DbUpdateConcurrencyException">
    /// A concurrency violation is encountered while saving to the database.
    /// A concurrency violation occurs when an unexpected number of rows are affected during save.
    /// This is usually because the data in the database has been modified since it was loaded into memory.
    /// </exception>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute an action within a transaction.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="loggingContext">Context for logging information.</param>
    /// <returns>A result with success information about the transaction.</returns>
    Result UseTransaction(
        Action action,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    );

    /// <summary>
    /// Execute an action within a transaction and return a value.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="loggingContext">Context for logging information.</param>
    /// <typeparam name="T">The type of the value returned.</typeparam>
    /// <returns>A result of type <see cref="T"/> with success information about the transaction.</returns>
    Result<T> UseTransaction<T>(
        Func<T> action,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    );

    /// <summary>
    /// Execute an action within a transaction and return a value. Expects the flow to be controlled with results instead of exceptions.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="loggingContext">Context for logging information.</param>
    /// <typeparam name="T">The type of the value returned wrapped within a <see cref="Result{TValue}"/>.</typeparam>
    /// <returns>A result of type <see cref="T"/> with success information about the transaction.</returns>
    Result<T> UseTransaction<T>(
        Func<Result<T>> action,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    );

    /// <summary>
    /// Execute an action within a transaction.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="loggingContext">Context for logging information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous result with information about the success of the transaction.
    /// </returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task<Result> UseTransactionAsync(
        Func<Task> action,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    );

    /// <summary>
    /// Execute an action within a transaction and return a value.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="loggingContext">Context for logging information.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A task that represents the asynchronous result of type <see cref="T"/>
    /// with information about the success of the transaction.
    /// </returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task<Result<T>> UseTransactionAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    );

    /// <summary>
    /// Execute an action within a transaction and return a value. Expects the flow to be controlled with results instead of exceptions.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="loggingContext">Context for logging information.</param>
    /// <typeparam name="T">The type of the value returned wrapped within a <see cref="Result{TValue}"/>.</typeparam>
    /// <returns>A result of type <see cref="T"/> with success information about the transaction.</returns>
    Task<Result<T>> UseTransaction<T>(
        Func<Task<Result<T>>> action,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    );
}
