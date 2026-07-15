using MediatR;
using Microsoft.AspNetCore.Http;
using Web.Library.Application.Features.Uploader;

namespace Web.Public.Features.Uploader
{
    public sealed record UploadDocumentCommand(
        IFormFile File
    ) : IRequest<FileMetadata>;
}
