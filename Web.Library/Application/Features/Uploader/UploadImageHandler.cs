using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Web.Library.Application.Abstractions;
using Web.Library.Domain;

namespace Web.Library.Application.Features.Uploader
{
    public class UploadImageHandler : IRequestHandler<UploadImageCommand, FileMetadata>
    {

        private readonly IStoredFileRepository _storedFileRepository;
        private readonly IFileStorage _fileStorage;
        private readonly IObjectKeyGenerator _objectKeyGenerator;
        private readonly IClock _clock;
        private readonly ILogger _logger;

        public UploadImageHandler(IStoredFileRepository storedFileRepository, IFileStorage fileStorage, IObjectKeyGenerator objectKeyGenerator, IClock clock)
        {
            _storedFileRepository = storedFileRepository;
            _fileStorage = fileStorage;
            _objectKeyGenerator = objectKeyGenerator;
            _clock = clock;
        }

        public async Task<FileMetadata> Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var id = Guid.CreateVersion7();
                var fileName = request.File.FileName;
                var extension = Path.GetExtension(request.File.FileName);
                var sizeInByte = request.File.Length;
                var contentType = request.File.ContentType;
                var objectKey = _objectKeyGenerator.Generate(ObjectKeyPrefixes.ProductImage, id, extension);
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
                    category: StoredFileCategory.Image,
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
