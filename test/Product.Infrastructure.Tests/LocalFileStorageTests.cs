using System.Text;
using Microsoft.Extensions.Options;
using Web.Public.Storage;

namespace Storage.Tests
{
    [TestClass]
    public sealed class LocalFileStorageTests
    {
        private string _rootDirectory = default!;
        private LocalFileStorage _storage = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            _rootDirectory = Path.Combine(
                Path.GetTempPath(),
                "RigorTools_LocalFileStorageTests",
                Guid.NewGuid().ToString("N"));

            Directory.CreateDirectory(_rootDirectory);

            var options = Options.Create(new FileStorageOptions
            {
                RootPath = _rootDirectory
            });

            _storage = new LocalFileStorage(options);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (Directory.Exists(_rootDirectory))
            {
                Directory.Delete(_rootDirectory, recursive: true);
            }
        }

        [TestMethod]
        public async Task SaveAsync_ShouldCreateTxtFile()
        {
            const string objectKey = "note.txt";
            const string expectedContent = "saved text content";

            await using var content = CreateTextStream(expectedContent);

            await _storage.SaveAsync(objectKey, content, CancellationToken.None);

            var savedPath = Path.Combine(_rootDirectory, objectKey);
            Assert.IsTrue(File.Exists(savedPath));
            Assert.AreEqual(expectedContent, await File.ReadAllTextAsync(savedPath));
        }

        [TestMethod]
        public async Task OpenReadAsync_ShouldReadSavedTxtFile()
        {
            const string objectKey = "read-me.txt";
            const string expectedContent = "read text content";

            await using var content = CreateTextStream(expectedContent);
            await _storage.SaveAsync(objectKey, content, CancellationToken.None);

            await using var stream = await _storage.OpenReadAsync(objectKey, CancellationToken.None);

            Assert.IsNotNull(stream);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var actualContent = await reader.ReadToEndAsync();
            Assert.AreEqual(expectedContent, actualContent);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldDeleteTxtFile()
        {
            const string objectKey = "delete-me.txt";

            await using var content = CreateTextStream("delete text content");
            await _storage.SaveAsync(objectKey, content, CancellationToken.None);

            var savedPath = Path.Combine(_rootDirectory, objectKey);
            Assert.IsTrue(File.Exists(savedPath));

            await _storage.DeleteAsync(objectKey, CancellationToken.None);

            Assert.IsFalse(File.Exists(savedPath));
        }

        private static MemoryStream CreateTextStream(string content)
            => new(Encoding.UTF8.GetBytes(content));
    }
}
