namespace Web.Library.Application.Features.Uploader
{
    public sealed record FileMetadata
    (
        Guid Id,
        string Name,
        long Length,
        string ContentType,
        DateTimeOffset UploadAt
    );
}
