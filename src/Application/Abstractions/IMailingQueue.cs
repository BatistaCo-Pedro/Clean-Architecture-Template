namespace Application.Abstractions;

/// <summary>
/// Interface for a mailing queue.
/// </summary>
public interface IMailingQueue
{
    /// <summary>
    /// Enqueues an email to be sent.
    /// </summary>
    /// <param name="emailInfoDto">The email info object containing all the info sent by the caller.</param>
    /// <returns>A job identifier for the created scheduled email.</returns>
    public NonEmptyString EnqueueEmail(EmailInfoDto emailInfoDto);

    /// <summary>
    /// Enqueues an email to be sent after a certain amount of time.
    /// </summary>
    /// <param name="emailInfoDto">The email info object containing all the info sent by the caller.</param>
    /// <param name="sendAt"><see cref="DateTimeOffset"/> defining when to send the email.</param>
    /// <returns>A job identifier for the created scheduled email.</returns>
    public NonEmptyString EnqueueScheduledEmail(EmailInfoDto emailInfoDto, DateTimeOffset sendAt);

    /// <summary>
    /// Adds a recurring email to the queue. Recurring emails are sent according to the cron expression passed in.
    /// </summary>
    /// <param name="jobId">A job identifier.</param>
    /// <param name="emailInfoDto">The email info object containing all the info sent by the caller.</param>
    /// <param name="cronExpression">The cron expression detailing when an email should be sent.</param>
    public void AddRecurringEmail(
        NonEmptyString jobId,
        EmailInfoDto emailInfoDto,
        NonEmptyString cronExpression
    );

    /// <summary>
    /// Dequeues a scheduled email.
    /// </summary>
    /// <param name="jobId">The job identifier to dequeue.</param>
    /// <returns>A boolean defining the success of the operation.</returns>
    /// <remarks>
    /// The job is not actually being deleted, this method changes only its state.
    /// This operation does not provide guarantee that the job will not be performed.
    /// If you are deleting a job that is performing right now, it will be performed anyway, despite of this call.
    /// The method returns result of a state transition. It can be false if a job was expired,
    /// its method does not exist or there was an exception during the state change process.
    /// </remarks>
    public bool DequeueScheduledEmail(NonEmptyString jobId);
}
