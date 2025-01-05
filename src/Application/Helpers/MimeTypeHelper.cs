namespace Application.Helpers;

public static class MimeTypeHelper
{
    public static NonEmptyString GetMimeType(NonEmptyString data) =>
        data.Value.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
            ? data.Value.Split(';')[0][5..]
            : throw new ArgumentException("The media type could not be determined.");

    public static NonEmptyString GetMimeType(Uri uri) =>
        new FileExtensionContentTypeProvider().TryGetContentType(uri.AbsolutePath, out var mimeType)
            ? mimeType
            : throw new ArgumentException("The media type could not be determined.");
}
