using FluentValidation;
using System.Net.Mime;
using Web.Public.Features.Uploader.Policies;

namespace Web.Public.Features.Uploader
{
    public class UploadDocumentCommandValidator: AbstractValidator<UploadDocumentCommand>
    {
        private readonly UploadPolicy _imagePolicy = new UploadPolicy()
        { 
            Category = UploadCategory.Document,
            MaxSizeInBytes = 5 * 1024 * 1024, // 5 MB
            AllowedContentTypes = new HashSet<string>
            {
                "application/msword", // .doc
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // .docx
                "application/pdf", // .pdf
                "application/vnd.ms-excel", // .xls
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" // xlsx
            }
        };

        public UploadDocumentCommandValidator() 
        {
            RuleFor(x=> x.File)
                .NotNull()
                    .WithMessage("No file were uploaded.")
                .Must(file => _imagePolicy.IsAllowed(file.ContentType, file.Length))
                    .WithMessage($"Unsupported file type within {_imagePolicy.Category}.");
        }
    }
}
