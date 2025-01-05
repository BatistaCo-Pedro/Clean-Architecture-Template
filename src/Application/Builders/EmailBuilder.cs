namespace Application.Builders;

/// <summary>
/// Builder class for emails.
/// </summary>
public sealed class EmailBuilder
{
    /// <summary>
    /// Underlying mail message. Set in the constructor.
    /// </summary>
    private readonly MailMessage _mailMessage;

    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="from">The sender of the email.</param>
    /// <param name="to">The recipient of the email.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="priority">The priority of the email.</param>
    public EmailBuilder(
        MailAddress from,
        MailAddress to,
        string subject,
        MailPriority priority = MailPriority.Normal
    ) =>
        _mailMessage = new MailMessage(from, to)
        {
            Subject = subject,
            Priority = priority,
            BodyEncoding = Encoding.UTF8
        };

    /// <summary>
    /// Adds an html body to the email.
    /// </summary>
    /// <param name="htmlBody">The html body to add to the email.</param>
    /// <param name="htmlBodyBuilderAction">An <see cref="HtmlBodyBuilder"/> to configure the html body.</param>
    /// <returns>A reference to the builder.</returns>
    public EmailBuilder AddHtml(
        HtmlString htmlBody,
        Action<HtmlBodyBuilder>? htmlBodyBuilderAction = null
    )
    {
        if (ContainsViewOfType(MediaTypeNames.Text.Html))
        {
            return this;
        }

        var htmlBodyBuilder = new HtmlBodyBuilder(htmlBody);
        htmlBodyBuilderAction?.Invoke(htmlBodyBuilder);
        var htmlView = htmlBodyBuilder.Build();

        _mailMessage.AlternateViews.Add(htmlView);
        _mailMessage.IsBodyHtml = true;
        return this;
    }

    /// <summary>
    /// Adds a plain text body to the email.
    /// </summary>
    /// <param name="plainBody">The plain text body to add.</param>
    /// <returns>A reference to the builder.</returns>
    public EmailBuilder AddPlain(NonEmptyString plainBody)
    {
        if (ContainsViewOfType(MediaTypeNames.Text.Plain))
        {
            return this;
        }

        var plainView = AlternateView.CreateAlternateViewFromString(
            plainBody,
            Encoding.UTF8,
            MediaTypeNames.Text.Plain
        );

        if (ContainsViewOfType(MediaTypeNames.Text.Html))
        {
            AddPlainViewInOrder(plainView);
            return this;
        }

        _mailMessage.AlternateViews.Add(plainView);
        return this;
    }

    /// <summary>
    /// Add attachments to the email.
    /// </summary>
    /// <param name="attachments">The attachment list to add.</param>
    /// <returns>A reference to the builder.</returns>
    public EmailBuilder AddAttachments(ICollection<Attachment> attachments)
    {
        foreach (var attachment in attachments)
        {
            _mailMessage.Attachments.Add(attachment);
        }
        return this;
    }

    /// <summary>
    /// Adds reply to addresses to the email.
    /// </summary>
    /// <param name="replyToArray">The array of reply to addresses.</param>
    /// <returns>A reference to the builder.</returns>
    public EmailBuilder AddReplyTo(params MailAddress[] replyToArray)
    {
        foreach (var replyTo in replyToArray)
        {
            _mailMessage.ReplyToList.Add(replyTo);
        }
        return this;
    }

    /// <summary>
    /// Decision-making method. Decides what email type to build and builds it.
    /// </summary>
    /// <returns>Email of type <see cref="MailMessage"/>.</returns>
    public MailMessage Build()
    {
        return _mailMessage;
    }

    /// <summary>
    /// Checks whether the mail message contains a view of a certain media type.
    /// </summary>
    /// <param name="mediaTypeName">The media type name - use <see cref="MediaTypeNames"/> to get the type.</param>
    /// <returns>A bool defining whether the mail message contains a view of the provided media type.</returns>
    private bool ContainsViewOfType(string mediaTypeName) =>
        _mailMessage.AlternateViews.Any(x => x.ContentType.MediaType == mediaTypeName);

    /// <summary>
    /// Mail messages send the last view to be added as main view, we want this view to be the html view.
    /// </summary>
    /// <param name="plainView">The plain view to add.</param>
    private void AddPlainViewInOrder(AlternateView plainView)
    {
        var htmlView = _mailMessage.AlternateViews[0];
        _mailMessage.AlternateViews.Remove(htmlView);
        _mailMessage.AlternateViews.Add(plainView);
        _mailMessage.AlternateViews.Add(htmlView);
    }

    /// <summary>
    /// Builder class for the html body of an email.
    /// </summary>
    /// <param name="htmlBody">The html body to create a view from.</param>
    public sealed class HtmlBodyBuilder(HtmlString htmlBody)
    {
        private readonly AlternateView _htmlBody = AlternateView.CreateAlternateViewFromString(
            htmlBody,
            Encoding.UTF8,
            MediaTypeNames.Text.Html
        );

        /// <summary>
        /// Add linked resources to the html view.
        /// </summary>
        /// <param name="linkedResources">The list of linked resources to add.</param>
        /// <returns>A reference to the <see cref="HtmlBodyBuilder"/>.</returns>
        public HtmlBodyBuilder AddLinkedResources(ICollection<LinkedResource> linkedResources)
        {
            foreach (var linkedResource in linkedResources)
            {
                _htmlBody.LinkedResources.Add(linkedResource);
            }
            return this;
        }

        /// <summary>
        /// Build the html view.
        /// </summary>
        /// <returns>An HTML view as <see cref="AlternateView"/>.</returns>
        public AlternateView Build()
        {
            return _htmlBody;
        }
    }
}
