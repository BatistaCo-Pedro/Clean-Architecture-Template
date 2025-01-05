namespace Infrastructure.Persistence.Converters;

/// <summary>
/// Converts <see cref="NonEmptyString"/> to <see cref="string"/> and vice versa for the database.
/// </summary>
internal sealed class NonEmptyStringConverter : ValueConverter<NonEmptyString, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NonEmptyStringConverter"/> class.
    /// </summary>
    public NonEmptyStringConverter()
        : base(
            nonEmptyString => (string)nonEmptyString,
            stringValue => new NonEmptyString(stringValue)
        ) { }
}
