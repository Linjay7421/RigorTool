using MediatR;
using Web.Public.Features.Uploader.Models;

namespace Web.Public.Features.Uploader
{
    public sealed record UploadDocumentCommand(
        IFormFile File
    ) : IRequest<FileMetadata>;
}
