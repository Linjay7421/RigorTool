using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Web.Library.Application.Abstractions;
using Web.Library.Application.Features.Uploader;
using Web.Library.Domain;

namespace Handlers.Tests
{
    [TestClass]
    public sealed class UploadImageHandlerTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public async Task Handler_ShouldStoreImageAndReturnMetadata()
        {
            // Arrange
            var fileContent = new byte[] { 1, 2, 3, 4, 5 };
            var file = UploadHandlerTestHelpers.CreateFormFile("photo.png", "image/png", fileContent);
            var storedFileRepository = new Mock<IStoredFileRepository>();
            var fileStorage = new Mock<IFileStorage>();
            var objectKeyGenerator = new Mock<IObjectKeyGenerator>();
            var clock = new UploadHandlerTestHelpers.TestClock(UploadHandlerTestHelpers.FixedNow);
            StoredFile? capturedStoredFile = null;
            byte[]? savedContent = null;
            
            // Mock fake datas
            objectKeyGenerator
                .Setup(x => x.Generate(ObjectKeyPrefixes.ProductImage, It.IsAny<Guid>(), ".png"))
                .Returns<string, Guid, string>((prefix, id, extension) => $"{prefix}/{id}{extension}");

            fileStorage
                .Setup(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Callback<string, Stream, CancellationToken>((_, content, _) =>
                {
                    using var saved = new MemoryStream();
                    content.CopyTo(saved);
                    savedContent = saved.ToArray();
                })
                .Returns(Task.CompletedTask);

            storedFileRepository
                .Setup(x => x.AddAsync(It.IsAny<StoredFile>(), It.IsAny<CancellationToken>()))
                .Callback<StoredFile, CancellationToken>((storedFile, _) => capturedStoredFile = storedFile)
                .Returns(Task.CompletedTask);

            // DI arrange
            var handler = new UploadImageHandler(
                storedFileRepository.Object,
                fileStorage.Object,
                objectKeyGenerator.Object,
                clock);

            // Act
            var result = await handler.Handle(new UploadImageCommand(file), CancellationToken.None);

            // Assert
            Assert.IsNotNull(capturedStoredFile);
            Assert.AreEqual(result.Id, capturedStoredFile.Id);
            Assert.AreEqual("photo.png", result.Name);
            Assert.AreEqual(fileContent.Length, result.Length);
            Assert.AreEqual("image/png", result.ContentType);
            Assert.AreEqual(UploadHandlerTestHelpers.FixedNow, result.UploadAt);

            Assert.AreEqual("photo.png", capturedStoredFile.OriginalName);
            Assert.AreEqual(StoredFileCategory.Image, capturedStoredFile.Category);
            Assert.AreEqual("image/png", capturedStoredFile.MimeType);
            Assert.AreEqual("png", capturedStoredFile.Extension);
            Assert.AreEqual(fileContent.Length, capturedStoredFile.SizeInBytes);
            Assert.AreEqual(UploadHandlerTestHelpers.ComputeSha256(fileContent), capturedStoredFile.ContentHash);
            Assert.AreEqual($"{ObjectKeyPrefixes.ProductImage}/{capturedStoredFile.Id}.png", capturedStoredFile.ObjectKey);
            Assert.AreEqual(UploadHandlerTestHelpers.FixedNow, capturedStoredFile.CreatedAt);
            CollectionAssert.AreEqual(fileContent, savedContent);

            fileStorage.Verify(
                x => x.SaveAsync(capturedStoredFile.ObjectKey, It.IsAny<Stream>(), It.IsAny<CancellationToken>()),
                Times.Once);
            storedFileRepository.Verify(
                x => x.AddAsync(It.IsAny<StoredFile>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public async Task Mediator_ShouldResolveHandler()
        {
            // Arrange
            var services = UploadHandlerTestHelpers.CreateUploaderServices();
            await using var provider = services.BuildServiceProvider();
            var mediator = provider.GetRequiredService<IMediator>();
            var file = UploadHandlerTestHelpers.CreateFormFile("photo.jpg", "image/jpeg", new byte[] { 10, 20, 30 });

            // Act
            var result = await mediator.Send(new UploadImageCommand(file), CancellationToken.None);

            // Assert
            Assert.AreEqual("photo.jpg", result.Name);
            Assert.AreEqual("image/jpeg", result.ContentType);
            provider.GetRequiredService<Mock<IStoredFileRepository>>()
                .Verify(x => x.AddAsync(It.Is<StoredFile>(file => file.Category == StoredFileCategory.Image), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
