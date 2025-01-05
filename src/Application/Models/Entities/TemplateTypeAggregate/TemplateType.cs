#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Application.Models.Entities.TemplateTypeAggregate;

/// <summary>
/// Template type entity.
/// </summary>
public class TemplateType : AggregateRoot
{
    [Obsolete("Required by DI and EF Core")]
    protected TemplateType() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateType"/> class.
    /// </summary>
    /// <param name="name">The name of the template type.</param>
    public TemplateType(NonEmptyString name)
    {
        Name = name;
    }

    /// <summary>
    /// Private list of email body contents for encapsulation.
    /// </summary>
    private readonly List<EmailTemplate> _emailTemplates = [];

    /// <summary>
    /// The name of the template type.
    /// </summary>
    public NonEmptyString Name { get; private set; }

    /// <summary>
    /// The email templates of this template type as <see cref="IReadOnlyCollection{T}"/> for encapsulation.
    /// </summary>
    public virtual IReadOnlyCollection<EmailTemplate> EmailTemplates
    {
        get => _emailTemplates;
        private init => _emailTemplates = value.ToList();
    }

    /// <summary>
    /// Gets a fallback email template.
    /// This will simply be the first email template that does not match the provided ID.
    /// </summary>
    /// <param name="emailTemplateIdToNotMatch">The ID to not match.</param>
    /// <returns>A <see cref="Result"/> representing the result of the operation.</returns>
    public Result<EmailTemplate> GetFallBackEmailTemplate(Guid emailTemplateIdToNotMatch) =>
        _emailTemplates
            .FirstOrDefault(x => x.Id != emailTemplateIdToNotMatch)
            .ToResult(
                $"Template type with ID {Id} does not contain a second email template to use as fallback"
            );

    /// <summary>
    /// Adds an email template to the template type.
    /// </summary>
    /// <param name="emailTemplate">The email template to add.</param>
    public void AddEmailTemplate(EmailTemplate emailTemplate)
    {
        _emailTemplates.Add(emailTemplate);

        RaiseDomainEvent(
            new EmailTemplateCreated(
                emailTemplate.Id,
                Id,
                _emailTemplates.Count == 1
            )
        );
    }
}
