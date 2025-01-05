namespace Application.Abstractions;

/// <summary>
/// Interface for the template type repository.
/// </summary>
public interface ITemplateTypeRepository : ICrudRepository<TemplateType>
{
    /// <summary>
    /// Gets an email template by its ID.
    /// </summary>
    /// <param name="emailTemplateId">The email template to get.</param>
    /// <returns>The <see cref="emailTemplateId"/> matching the ID.</returns>
    Result<EmailTemplate> GetEmailTemplate(Guid emailTemplateId);
}