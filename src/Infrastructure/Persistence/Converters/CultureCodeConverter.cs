namespace Infrastructure.Persistence.Converters;

/// <summary>
/// Converts <see cref="string"/> to <see cref="CultureCode"/> and vice versa for the database.
/// </summary>
internal sealed class CultureCodeConverter : ValueConverter<CultureCode, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CultureCodeConverter"/> class.
    /// </summary>
    public CultureCodeConverter()
        : base(
            nonEmptyString => (string)nonEmptyString,
            stringValue => new CultureCode((NonEmptyString)stringValue)
        ) { }
}
