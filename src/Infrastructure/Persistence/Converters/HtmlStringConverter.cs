namespace Infrastructure.Persistence.Converters;

/// <summary>
/// Converts <see cref="HtmlString"/> to <see cref="string"/> and vice versa for the database.
/// </summary>
internal sealed class HtmlStringConverter : ValueConverter<HtmlString, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlStringConverter"/> class.
    /// </summary>
    public HtmlStringConverter()
        : base(
            htmlString => (string)htmlString,
            stringValue => new HtmlString((NonEmptyString)stringValue)
        ) { }
}
