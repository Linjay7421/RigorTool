using Library.Infrastructure.IntergrationTests.Common;
using Microsoft.Extensions.Options;
using System.Text;
using Web.Public.Storage;

namespace Storage.Tests
{
    [TestClass]
    public sealed class LocalFileStorageTests
    {
        private string _rootDirectory = default!;
        private LocalFileStorage _storage = default!;
        private LocalFileStorageFixture _storageFixture = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _storageFixture = new LocalFileStorageFixture();
            _storage = new LocalFileStorage(_storageFixture.Options);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _storageFixture.Dispose();
        }

        [TestMethod]
        [TestCategory("Intergration")]
        public async Task SaveAsync_ShouldCreateTxtFile()
        {
            const string objectKey = "note.txt";
            const string expectedContent = "saved text content";

            await using var content = CreateTextStream(expectedContent);

            await _storage.SaveAsync(objectKey, content, CancellationToken.None);

            var savedPath = Path.Combine(_storageFixture.RootDirectory, objectKey);
            Assert.IsTrue(File.Exists(savedPath));
            Assert.AreEqual(expectedContent, await File.ReadAllTextAsync(savedPath));
        }

        [TestMethod]
        [TestCategory("Intergration")]
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
        [TestCategory("Intergration")]
        public async Task DeleteAsync_ShouldDeleteTxtFile()
        {
            const string objectKey = "delete-me.txt";

            await using var content = CreateTextStream("delete text content");
            await _storage.SaveAsync(objectKey, content, CancellationToken.None);

            var savedPath = Path.Combine(_storageFixture.RootDirectory, objectKey);
            Assert.IsTrue(File.Exists(savedPath));

            await _storage.DeleteAsync(objectKey, CancellationToken.None);

            Assert.IsFalse(File.Exists(savedPath));
        }

        private static MemoryStream CreateTextStream(string content)
            => new(Encoding.UTF8.GetBytes(content));
    }
}
