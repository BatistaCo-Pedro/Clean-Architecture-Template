using Application.Builders;

namespace Application.Services;

/// <summary>
/// Service for managing mailing operations.
/// </summary>
public interface IMailingService
{
    /// <summary>
    /// Gets a mail message based on the provided email info.
    /// </summary>
    /// <param name="emailInfoDto">The email info to create the mail message from.</param>
    /// <returns>A <see cref="Result{TValue}"/> of type <see cref="MailMessage"/> representing the result of the operation.</returns>
    Result<MailMessage> GetMailMessage(EmailInfoDto emailInfoDto);
}

/// <inheritdoc />
public class MailingService(IUnitOfWork unitOfWork) : IMailingService
{
    /// <inheritdoc />
    public Result<MailMessage> GetMailMessage(EmailInfoDto emailInfoDto)
    {
        var templateTypeRepository = unitOfWork.GetRepository<ITemplateTypeRepository>();

        return templateTypeRepository
            .GetEmailTemplate(emailInfoDto.EmailTemplateId)
            .Match(emailTemplate => CreateMailMessage(emailInfoDto, emailTemplate));
    }

    private static Result<MailMessage> CreateMailMessage(
        EmailInfoDto emailInfoDto,
        EmailTemplate emailTemplate
    )
    {
        return emailTemplate
            .GetContent(emailInfoDto.CultureCode)
            .Match(
                emailBodyContent =>
                    Result<MailMessage>.Ok(
                        new EmailBuilder(
                            emailInfoDto.SenderMailAddress,
                            emailInfoDto.RecipientMailAddress,
                            emailBodyContent.Subject
                        )
                            .AddPlain(emailBodyContent.Body.StripHtml())
                            .AddHtml(emailBodyContent.Body)
                            .Build()
                    )
            );
    }
}
