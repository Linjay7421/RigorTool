using FluentValidation;
using Web.Library.Application.Features.Uploader;

namespace Web.Public.Features.Uploader
{
    public class UploadImageCommandValidator: AbstractValidator<UploadImageCommand>
    {
        private readonly UploadPolicy _imagePolicy = new UploadPolicy()
        { 
            Category = UploadCategory.Image,
            MaxSizeInBytes = 5 * 1024 * 1024, // 5 MB
            AllowedContentTypes = new HashSet<string>
            {
                "image/png",
                "image/jpeg",
                "image/gif"
            }
        };

        public UploadImageCommandValidator() 
        {
            RuleFor(x=> x.File)
                .NotNull()
                    .WithMessage("No file were uploaded.")
                .Must(file => _imagePolicy.IsAllowed(file.ContentType, file.Length))
                    .WithMessage($"Unsupported file type within {_imagePolicy.Category}.");
        }
    }
}
