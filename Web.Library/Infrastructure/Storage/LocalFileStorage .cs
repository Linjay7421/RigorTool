using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Web.Library.Application.Abstractions;

namespace Web.Library.Infrastructure.Storage
{
    public enum FileNamespace
    {
        ProductImage,
        ProductDocument,
        UserAvatar,
        CampaignBanner
    }

    public sealed class LocalFileStorage: IFileStorage
    {
        private readonly IOptions<FileStorageOptions> _config;
        private readonly ILogger _logger;
        private readonly string _rootDirectory;

        public LocalFileStorage(IOptions<FileStorageOptions> config)
        { 
            _config = config;

            if (String.IsNullOrEmpty(_config.Value.RootPath)) 
                throw new ArgumentNullException();
            
            if (!CheckDirectoryExists(_config.Value.RootPath))
                throw new DirectoryNotFoundException();

            _rootDirectory = _config.Value.RootPath;
        }

        private string GetLocalPath(string objectKey)
        {
            var fileName = Path.GetFileName(objectKey);
            return Path.Combine(_rootDirectory, fileName);
        }

        private bool CheckFikeExists(string path)
            => File.Exists(path);

        private bool CheckDirectoryExists(string path)
            => Directory.Exists(path);

        private string ExtractFileName(string objectKey)
            => Path.GetFileName(objectKey);

        public async Task SaveAsync(string objectKey, Stream content, CancellationToken cancellationToken)
        {
            var path = GetLocalPath(ExtractFileName(objectKey));

            using (FileStream outputSteam = File.Create(path))
            {
                await content.CopyToAsync(outputSteam);
            }
        }

        public Task<Stream?> OpenReadAsync(string objectKey, CancellationToken cancellationToken)
        {
            var path = GetLocalPath(ExtractFileName(objectKey));

            if (!File.Exists(path))
                Task.FromResult<Stream?>(null);

            Stream stream = File.OpenRead(path);

            return Task.FromResult<Stream?>(stream);
        }

        public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken)
        {
            var path = GetLocalPath(ExtractFileName(objectKey));

            File.Delete(path);
        }
    }
}
