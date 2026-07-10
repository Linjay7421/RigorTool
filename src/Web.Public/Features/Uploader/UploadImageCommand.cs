using MediatR;
using Web.Public.Features.Uploader.Models;

namespace Web.Public.Features.Uploader
{
    public sealed record UploadImageCommand(
        IFormFile File
    ) : IRequest<FileMetadata>;
}
