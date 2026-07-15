using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Web.Public.Storage;

namespace Library.Infrastructure.IntergrationTests.Common
{
    public sealed class LocalFileStorageFixture : IDisposable
    {
        public string RootDirectory { get; }

        public IOptions<FileStorageOptions> Options { get; }

        public LocalFileStorageFixture()
        {
            RootDirectory = Path.Combine(
                Path.GetTempPath(),
                "RigorTools_LocalFileStorageTests",
                Guid.NewGuid().ToString("N"));

            Directory.CreateDirectory(RootDirectory);

            Options = Microsoft.Extensions.Options.Options.Create(
                new FileStorageOptions
                {
                    RootPath = RootDirectory
                });
        }

        public void Dispose()
        {
            if (Directory.Exists(RootDirectory))
            {
                Directory.Delete(RootDirectory, true);
            }
        }

        public static FormFile CreateFormFile(string fileName, string contentType, byte[] content)
        {
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }
    }
}
