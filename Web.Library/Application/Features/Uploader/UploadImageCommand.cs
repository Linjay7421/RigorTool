using MediatR;
using Microsoft.AspNetCore.Http;

namespace Web.Library.Application.Features.Uploader
{
    public sealed record UploadImageCommand(
        IFormFile File
    ) : IRequest<FileMetadata>;
}
