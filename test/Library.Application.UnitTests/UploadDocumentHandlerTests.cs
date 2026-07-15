using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Web.Library.Application.Abstractions;
using Web.Library.Application.Features.Uploader;
using Web.Library.Domain;

namespace Handlers.Tests
{
    [TestClass]
    public sealed class UploadDocumentHandlerTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public async Task Handle_ShouldStoreDocumentAndReturnMetadata()
        {
            // Arrange
            var fileContent = "document payload"u8.ToArray();
            var file = UploadHandlerTestHelpers.CreateFormFile("brief.pdf", "application/pdf", fileContent);
            var storedFileRepository = new Mock<IStoredFileRepository>();
            var fileStorage = new Mock<IFileStorage>();
            var objectKeyGenerator = new Mock<IObjectKeyGenerator>();
            var clock = new UploadHandlerTestHelpers.TestClock(UploadHandlerTestHelpers.FixedNow);
            StoredFile? capturedStoredFile = null;
            byte[]? savedContent = null;

            objectKeyGenerator
                .Setup(x => x.Generate(ObjectKeyPrefixes.ProductDocument, It.IsAny<Guid>(), ".pdf"))
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

            var handler = new UploadDocumentHandler(
                storedFileRepository.Object,
                fileStorage.Object,
                objectKeyGenerator.Object,
                clock,
                NullLogger<UploadDocumentHandler>.Instance);

            // Act
            var result = await handler.Handle(new UploadDocumentCommand(file), CancellationToken.None);

            // Assert
            Assert.IsNotNull(capturedStoredFile);
            Assert.AreEqual(result.Id, capturedStoredFile.Id);
            Assert.AreEqual("brief.pdf", result.Name);
            Assert.AreEqual(fileContent.Length, result.Length);
            Assert.AreEqual("application/pdf", result.ContentType);
            Assert.AreEqual(UploadHandlerTestHelpers.FixedNow, result.UploadAt);

            Assert.AreEqual("brief.pdf", capturedStoredFile.OriginalName);
            Assert.AreEqual(StoredFileCategory.Document, capturedStoredFile.Category);
            Assert.AreEqual("application/pdf", capturedStoredFile.MimeType);
            Assert.AreEqual("pdf", capturedStoredFile.Extension);
            Assert.AreEqual(fileContent.Length, capturedStoredFile.SizeInBytes);
            Assert.AreEqual(UploadHandlerTestHelpers.ComputeSha256(fileContent), capturedStoredFile.ContentHash);
            Assert.AreEqual($"{ObjectKeyPrefixes.ProductDocument}/{capturedStoredFile.Id}.pdf", capturedStoredFile.ObjectKey);
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
            var file = UploadHandlerTestHelpers.CreateFormFile("spec.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", new byte[] { 40, 50, 60 });

            // Act
            var result = await mediator.Send(new UploadDocumentCommand(file), CancellationToken.None);

            // Assert
            Assert.AreEqual("spec.docx", result.Name);
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.wordprocessingml.document", result.ContentType);
            provider.GetRequiredService<Mock<IStoredFileRepository>>()
                .Verify(x => x.AddAsync(It.Is<StoredFile>(file => file.Category == StoredFileCategory.Document), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
