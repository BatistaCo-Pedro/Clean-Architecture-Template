namespace Infrastructure.Repositories;

internal class TemplateTypeRepository(SomeContext db) : BaseCrudRepository<TemplateType>(db), ITemplateTypeRepository
{
    /// <inheritdoc />
    public Result<EmailTemplate> GetEmailTemplate(Guid emailTemplateId) =>
        GetByCondition(x => x.EmailTemplates.Any(y => y.Id == emailTemplateId))
            .SelectMany(x => x.EmailTemplates)
            .FirstOrDefault(x => x.Id == emailTemplateId)
            .ToResult($"EmailTemplate with ID: `{emailTemplateId}` not found.");
}