using MediatR;
using System.Net.Mime;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Xml.Linq;
using Web.Public.Common.Abstraction;
using Web.Public.Domains;
using Web.Public.Features.Category.Models;
using Web.Public.Features.Uploader.Models;
using Web.Public.Repository;
using Web.Public.Storage;

namespace Web.Public.Features.Uploader
{
    public class UploadDocumentHandler : IRequestHandler<UploadDocumentCommand, FileMetadata>
    {

        private readonly IStoredFileRepository _storedFileRepository;
        private readonly IFileStorage _fileStorage;
        private readonly IObjectKeyGenerator _objectKeyGenerator;
        private readonly IClock _clock;
        private readonly ILogger<UploadDocumentHandler> _logger;

        public UploadDocumentHandler(IStoredFileRepository storedFileRepository, IFileStorage fileStorage, IObjectKeyGenerator objectKeyGenerator, IClock clock, ILogger<UploadDocumentHandler> logger)
        {
            _storedFileRepository = storedFileRepository;
            _fileStorage = fileStorage;
            _objectKeyGenerator = objectKeyGenerator;
            _clock = clock;
            _logger = logger;
        }

        public async Task<FileMetadata> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var id = Guid.CreateVersion7();
                var fileName = request.File.FileName;
                var extension = Path.GetExtension(request.File.FileName);
                var sizeInByte = request.File.Length;
                var contentType = request.File.ContentType;
                var objectKey = _objectKeyGenerator.Generate(ObjectKeyPrefixes.ProductDocument, id, extension);
                var createdAt = _clock.UtcNow;
                string contentHash;

                await using (var fileStream = request.File.OpenReadStream())
                {
                    using var sha = SHA256.Create();

                    var hashValue = await sha.ComputeHashAsync(fileStream, cancellationToken);
                    contentHash = Convert.ToHexString(hashValue).ToLowerInvariant();

                    fileStream.Position = 0; // Init reader back to start.

                    await _fileStorage.SaveAsync(objectKey, fileStream, cancellationToken);
                }

                var storedFile = StoredFile.Create(
                    id: id,
                    originalName: fileName,
                    category: StoredFileCategory.Document,
                    mimeType: contentType,
                    extension: extension,
                    sizeInBytes: sizeInByte,
                    contentHash: contentHash,
                    objectKey: objectKey,
                    createdAt: createdAt
                );

                // Add file to record.
                await _storedFileRepository.AddAsync(storedFile, cancellationToken);

                return new FileMetadata(
                    Id: id,
                    Name: fileName,
                    Length: sizeInByte,
                    ContentType: contentType,
                    UploadAt: createdAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
