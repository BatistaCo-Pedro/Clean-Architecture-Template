// ReSharper disable UnusedMember.Local
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Application.Models.Entities.TemplateTypeAggregate;

/// <summary>
/// Email template entity.
/// </summary>
public class EmailTemplate : AuditableEntity
{
    [Obsolete("Required by DI and EF Core")]
    protected EmailTemplate() { }

    /// <summary>
    /// Ctor. Creates an instance of <see cref="EmailTemplate"/>.
    /// </summary>
    /// <param name="name">A human-readable name for the template.</param>
    /// <param name="emailBodyContent">The first content to add.</param>
    /// <param name="templateTypeId">The ID of the type of the template.</param>
    /// <remarks>
    /// Email templates always have at least one email content body.
    /// </remarks>
    public EmailTemplate(
        NonEmptyString name,
        EmailBodyContent emailBodyContent,
        Guid templateTypeId
    )
    {
        Name = name;
        TemplateTypeId = templateTypeId;
        AddContent(emailBodyContent);
    }

    /// <summary>
    /// Private list of email body contents for encapsulation.
    /// </summary>
    private readonly List<EmailBodyContent> _emailBodyContents = [];

    /// <summary>
    /// Human-readable name of the template.
    /// </summary>
    public NonEmptyString Name { get; private set; }

    /// <summary>
    /// The email body contents of this template as <see cref="IReadOnlyCollection{T}"/> for encapsulation.
    /// </summary>
    public virtual IReadOnlyCollection<EmailBodyContent> EmailBodyContents
    {
        get => _emailBodyContents;
        private init => _emailBodyContents = value.ToList();
    }

    /// <summary>
    /// The type of the template.
    /// </summary>
    public virtual TemplateType TemplateType { get; private set; }

    /// <summary>
    /// The ID of the template type.
    /// </summary>
    /// <remarks>
    /// Foreign key configuration done in the context file.
    /// </remarks>
    public Guid TemplateTypeId { get; private set; }

    /// <summary>
    /// Gets the email body for the given culture code.
    /// </summary>
    /// <param name="cultureCode">The culture code to get the email body for.</param>
    /// <returns>
    /// A <see cref="EmailBodyContent"/> belonging to the email template matching the provided culture code.
    /// </returns>
    /// <exception cref="ArgumentException">Body couldn't be found for provided culture code.</exception>
    public Result<EmailBodyContent> GetContent(CultureCode cultureCode)
    {
        return Result<EmailBodyContent>.Try(
            () =>
                _emailBodyContents.FirstOrDefault(x => x.CultureCode == cultureCode)
                ?? throw new ArgumentException(
                    $"Couldn't find a body for the culture code {cultureCode}",
                    nameof(cultureCode)
                )
        );
    }

    private Result AddContent(EmailBodyContent emailBodyContent)
    {
        ChangeDefaultContent(emailBodyContent);
        _emailBodyContents.Add(emailBodyContent);

        return Result.Ok();
    }

    private void ChangeDefaultContent(EmailBodyContent emailBodyContent)
    {
        if (_emailBodyContents.Count == 0)
        {
            emailBodyContent.SetDefault(true);
        }
        else if (emailBodyContent.IsDefault)
        {
            _emailBodyContents.First(x => x.IsDefault).SetDefault(false);
        }
    }
}
