namespace Application.Models.Entities.TemplateTypeAggregate.Handlers;

/// <summary>
/// Event handler for <see cref="EmailTemplateCreated"/>.
/// </summary>
/// <param name="unitOfWork">The unit of work.</param>
/// <remarks>
/// This gets called right before save changes. Do not call <see cref="IUnitOfWork.SaveChanges"/> on here.
/// </remarks>
public class EmailTemplateCreatedHandler(IUnitOfWork unitOfWork)
    : IEventHandler<EmailTemplateCreated>
{
    /// <inheritdoc />
    public Task Handle(EmailTemplateCreated eventMessage)
    {
        // Do something here
        return Task.CompletedTask;
    }
}
