using System.Security.Cryptography;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Library.Application.Abstractions;
using Web.Library.Application.Behaviors;
using Web.Library.Application.Features.Uploader;
using Web.Library.Domain;

namespace Handlers.Tests
{
    internal static class UploadHandlerTestHelpers
    {
        public static readonly DateTimeOffset FixedNow = new(2026, 7, 10, 12, 0, 0, TimeSpan.Zero);

        public static ServiceCollection CreateUploaderServices()
        {
            var services = new ServiceCollection();
            var storedFileRepository = new Mock<IStoredFileRepository>();
            var fileStorage = new Mock<IFileStorage>();
            var objectKeyGenerator = new Mock<IObjectKeyGenerator>();

            storedFileRepository
                .Setup(x => x.AddAsync(It.IsAny<StoredFile>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            fileStorage
                .Setup(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            objectKeyGenerator
                .Setup(x => x.Generate(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns<string, Guid, string>((prefix, id, extension) => $"{prefix}/{id}{extension}");

            services.AddSingleton(storedFileRepository);
            services.AddSingleton(fileStorage);
            services.AddSingleton(objectKeyGenerator);
            services.AddSingleton<IStoredFileRepository>(storedFileRepository.Object);
            services.AddSingleton<IFileStorage>(fileStorage.Object);
            services.AddSingleton<IObjectKeyGenerator>(objectKeyGenerator.Object);
            services.AddSingleton<IClock>(new TestClock(FixedNow));
            services.AddScoped<IValidator<UploadImageCommand>, UploadImageCommandValidator>();
            services.AddScoped<IValidator<UploadDocumentCommand>, UploadDocumentCommandValidator>();

            services.AddMediatR(cfg =>
            {
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.RegisterServicesFromAssemblyContaining<UploadImageCommand>();
            });

            services.AddLogging(builder =>
            {
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            return services;
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

        public static string ComputeSha256(byte[] content)
        {
            var hash = SHA256.HashData(content);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        public sealed class TestClock(DateTimeOffset utcNow) : IClock
        {
            public DateTimeOffset UtcNow { get; } = utcNow;
        }
    }
}
