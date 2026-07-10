using FluentValidation;
using Library.Application.IntegrationTests.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Web.Public.Common.Abstraction;
using Web.Public.Common.Behaviors;
using Web.Public.Features.Uploader;
using Web.Public.Providers;
using Web.Public.Repository;
using Web.Public.Repository.Common;
using Web.Public.Storage;
namespace Handlers.tests
{
    [TestClass]
    public sealed class UploadImageHandlerTests
    {
        private ServiceProvider _provider = null!;
        private LocalFileStorageFixture _storageFixture = null!;
        private StorageDatabaseFixture _databaseFixture = null!;

        [TestInitialize]
        public void SetUp() 
        {
            // Fixtures
            _storageFixture = new LocalFileStorageFixture();
            _databaseFixture = new StorageDatabaseFixture();
            var services = new ServiceCollection();

            //Repository
            services.AddSingleton<IStorageDbConnectionFactory>(_databaseFixture.ConnectionFactory);
            services.AddScoped<IStoredFileRepository, RawSqlStoredFileRepository>();
            
            // Storage
            services.AddSingleton<IFileStorage>(new LocalFileStorage(_storageFixture.Options));
            
            // Providers
            services.AddSingleton<IClock, SystemClock>();
            services.AddSingleton<IObjectKeyGenerator, DefaultObjectKeyGenerator>();
            services.AddScoped<IValidator<UploadDocumentCommand>, UploadDocumentCommandValidator>();

            // Mediator
            services.AddMediatR(cfg =>
            {
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.RegisterServicesFromAssemblyContaining<UploadDocumentCommand>();
            });

            // Logger
            services.AddLogging(builder =>
            {
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            _provider = services.BuildServiceProvider();
        }

        [TestMethod]
        [TestCategory("Intergration")]
        public async Task Handler_shouldReturnSotredFileMeta()
        {
            // Arrange
            var mediator = _provider.GetRequiredService<IMediator>();
            var file = CreateFormFile("photo.jpg", "image/jpeg", new byte[] { 10, 20, 30 });

            // Act
            var result = await mediator.Send(new UploadImageCommand(file), CancellationToken.None);

            // Assert
            Assert.AreEqual("photo.jpg", result.Name);
            Assert.AreEqual("image/jpeg", result.ContentType);
        }

        [TestCleanup]
        public void cleanUp()
        {
            _storageFixture.Dispose();    
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
