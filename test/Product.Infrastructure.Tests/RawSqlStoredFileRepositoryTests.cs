using MySqlConnector;
using Web.Public.Domains;
using Web.Public.Repository;
using Web.Public.Repository.Common;

namespace Repository.Tests
{
    [TestClass]
    public sealed class RawSqlStoredFileRepositoryTests
    {
        private const string TestConnectionString =
            "Server=localhost;Port=13306;Database=FileDB;Uid=root;Pwd=MyStrongPass123!;";

        private RawSqlStoredFileRepository _repository = default!;

        [TestInitialize]
        public void TestInitialize()
        {
            var factory = new StorageDbConnectionFactory(TestConnectionString);
            _repository = new RawSqlStoredFileRepository(factory);
        }

        [TestMethod]
        public async Task AddAsync_ShouldPersistStoredFile()
        {
            var storedFile = CreateStoredFile();

            await _repository.AddAsync(storedFile, CancellationToken.None);

            var persisted = await GetStoredFileByIdAsync(storedFile.Id);
            Assert.IsNotNull(persisted);
            Assert.AreEqual(storedFile.OriginalName, persisted.OriginalName);
            Assert.AreEqual(storedFile.Category, persisted.Category);
            Assert.AreEqual(storedFile.MimeType, persisted.MimeType);
            Assert.AreEqual(storedFile.Extension, persisted.Extension);
            Assert.AreEqual(storedFile.SizeInBytes, persisted.SizeInBytes);
            Assert.AreEqual(storedFile.ObjectKey, persisted.ObjectKey);
            Assert.AreEqual(storedFile.ContentHash, persisted.ContentHash);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnStoredFile()
        {
            var storedFile = CreateStoredFile();
            await InsertStoredFileAsync(storedFile);

            var result = await _repository.GetByIdAsync(storedFile.Id, CancellationToken.None);

            AssertStoredFile(storedFile, result);
        }

        [TestMethod]
        public async Task GetByObjectKeyAsync_ShouldReturnStoredFile()
        {
            var storedFile = CreateStoredFile();
            await InsertStoredFileAsync(storedFile);

            var result = await _repository.GetByObjectKeyAsync(storedFile.ObjectKey, CancellationToken.None);

            AssertStoredFile(storedFile, result);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnNull_WhenStoredFileDoesNotExist()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByObjectKeyAsync_ShouldReturnNull_WhenStoredFileDoesNotExist()
        {
            var result = await _repository.GetByObjectKeyAsync($"missing/{Guid.NewGuid():N}.txt", CancellationToken.None);

            Assert.IsNull(result);
        }

        private static Web.Public.Domains.StoredFile CreateStoredFile()
        {
            var id = Guid.NewGuid();

            return Web.Public.Domains.StoredFile.Create(
                id: id,
                originalName: "test-file.txt",
                category: StoredFileCategory.Document,
                mimeType: "text/plain",
                extension: ".txt",
                sizeInBytes: 42,
                contentHash: "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
                objectKey: $"documents/{id:N}.txt",
                createdAt: new DateTimeOffset(2026, 7, 9, 0, 0, 0, TimeSpan.Zero));
        }

        private static async Task<Web.Public.Domains.StoredFile?> GetStoredFileByIdAsync(Guid id)
        {
            await using var connection = new MySqlConnection(TestConnectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT Id, OriginalName, Category, MimeType, Extension, SizeInBytes, ObjectKey, ContentHash, CreatedAt
                FROM Files
                WHERE Id = @Id
                LIMIT 1;
            ";

            await using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id.ToString());

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return ReadStoredFile(reader);
        }

        private static async Task InsertStoredFileAsync(Web.Public.Domains.StoredFile storedFile)
        {
            await using var connection = new MySqlConnection(TestConnectionString);
            await connection.OpenAsync();

            const string sql = @"
                INSERT INTO Files
                    (Id, OriginalName, Category, MimeType, Extension, SizeInBytes, ObjectKey, ContentHash, CreatedAt)
                VALUES
                    (@Id, @OriginalName, @Category, @MimeType, @Extension, @SizeInBytes, @ObjectKey, @ContentHash, @CreatedAt);
            ";

            await using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", storedFile.Id.ToString());
            command.Parameters.AddWithValue("@OriginalName", storedFile.OriginalName);
            command.Parameters.AddWithValue("@Category", (byte)storedFile.Category);
            command.Parameters.AddWithValue("@MimeType", storedFile.MimeType);
            command.Parameters.AddWithValue("@Extension", storedFile.Extension);
            command.Parameters.AddWithValue("@SizeInBytes", storedFile.SizeInBytes);
            command.Parameters.AddWithValue("@ObjectKey", storedFile.ObjectKey);
            command.Parameters.AddWithValue("@ContentHash", storedFile.ContentHash);
            command.Parameters.AddWithValue("@CreatedAt", storedFile.CreatedAt.UtcDateTime);

            await command.ExecuteNonQueryAsync();
        }

        private static Web.Public.Domains.StoredFile ReadStoredFile(MySqlDataReader reader)
            => Web.Public.Domains.StoredFile.Create(
                id: reader.GetGuid("Id"),
                originalName: reader.GetString("OriginalName"),
                category: (StoredFileCategory)reader.GetByte("Category"),
                mimeType: reader.GetString("MimeType"),
                extension: reader.GetString("Extension"),
                sizeInBytes: reader.GetInt64("SizeInBytes"),
                objectKey: reader.GetString("ObjectKey"),
                contentHash: reader.GetString("ContentHash"),
                createdAt: new DateTimeOffset(reader.GetDateTime("CreatedAt"), TimeSpan.Zero));

        private static void AssertStoredFile(
            Web.Public.Domains.StoredFile expected,
            Web.Public.Domains.StoredFile? actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.OriginalName, actual.OriginalName);
            Assert.AreEqual(expected.Category, actual.Category);
            Assert.AreEqual(expected.MimeType, actual.MimeType);
            Assert.AreEqual(expected.Extension, actual.Extension);
            Assert.AreEqual(expected.SizeInBytes, actual.SizeInBytes);
            Assert.AreEqual(expected.ObjectKey, actual.ObjectKey);
            Assert.AreEqual(expected.ContentHash, actual.ContentHash);
            Assert.AreEqual(expected.CreatedAt.UtcDateTime, actual.CreatedAt.UtcDateTime);
        }
    }
}
