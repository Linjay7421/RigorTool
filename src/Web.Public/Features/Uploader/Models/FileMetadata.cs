using Microsoft.AspNetCore.Http.HttpResults;

namespace Web.Public.Features.Uploader.Models
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
