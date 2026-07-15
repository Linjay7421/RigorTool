using MySqlConnector;
using System.Data;
using Web.Library.Application.Abstractions;
using Web.Library.Domain;
using Web.Library.Infrastructure.Repository.Common;

namespace Web.Library.Infrastructure.Persistence
{
    public sealed class RawSqlStoredFileRepository : IStoredFileRepository
    {
        private readonly IStorageDbConnectionFactory _connectionFactory;
        public RawSqlStoredFileRepository(IStorageDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(StoredFile file, CancellationToken cancellationToken)
        {
            // Open database connection.
            await using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync(cancellationToken);

                const string sql = @"
                    INSERT INTO Files
                        (Id, OriginalName, Category, MimeType, Extension, SizeInBytes, ObjectKey, ContentHash, CreatedAt)
                    VALUES
                        (@Id, @OriginalName, @Category, @MimeType, @Extension, @SizeInBytes, @ObjectKey, @ContentHash, @CreatedAt);
                ";

                // Execute SQL command.
                await using (var command = new MySqlCommand(sql, connection))
                {
                    // Set parameters 
                    command.Parameters.Add("@Id", MySqlDbType.Guid).Value = file.Id.ToString(); // Char(36)
                    command.Parameters.Add("@OriginalName", MySqlDbType.String).Value = file.OriginalName;
                    command.Parameters.Add("@Category", MySqlDbType.Byte).Value = file.Category;
                    command.Parameters.Add("@MimeType", MySqlDbType.String).Value = file.MimeType;
                    command.Parameters.Add("@Extension", MySqlDbType.String).Value = file.Extension;
                    command.Parameters.Add("@SizeInBytes", MySqlDbType.Int64).Value = file.SizeInBytes;
                    command.Parameters.Add("@ObjectKey", MySqlDbType.String).Value = file.ObjectKey;
                    command.Parameters.Add("@ContentHash", MySqlDbType.String).Value = file.ContentHash;
                    command.Parameters.Add("@CreatedAt", MySqlDbType.Timestamp).Value = file.CreatedAt.UtcDateTime;
                    // Execute
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }

        public async Task<StoredFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            // Open database connection.
            await using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync(cancellationToken);

                const string sql = @"
                    SELECT * 
                    FROM Files f
                    WHERE f.Id = @Id
                    LIMIT 1;
                ";

                // Execute SQL command.
                await using (var command = new MySqlCommand(sql, connection))
                {
                    // Set parameters 
                    command.Parameters.Add("@Id", MySqlDbType.Guid).Value = id.ToString();
                    // Execute
                    await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        if (!await reader.ReadAsync(cancellationToken))
                            return null;
                        return StoredFile.Create(
                            id: reader.GetGuid("Id"),
                            originalName: reader.GetString("OriginalName"),
                            category: (StoredFileCategory)reader.GetByte("Category"),
                            mimeType: reader.GetString("MimeType"),
                            extension: reader.GetString("Extension"),
                            sizeInBytes: reader.GetInt64("SizeInBytes"),
                            objectKey: reader.GetString("ObjectKey"),
                            contentHash: reader.GetString("ContentHash"),
                            createdAt: new DateTimeOffset(reader.GetDateTime("CreatedAt"), TimeSpan.Zero)
                        );
                    }
                }
            }
        }

        public async Task<StoredFile?> GetByObjectKeyAsync(string objectKey, CancellationToken cancellationToken)
        {
            // Open database connection.
            await using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync(cancellationToken);

                const string sql = @"
                    SELECT * 
                    FROM Files f
                    WHERE f.ObjectKey = @ObjectKey
                    LIMIT 1;
                ";

                // Execute SQL command.
                await using (var command = new MySqlCommand(sql, connection))
                {
                    // Set parameters 
                    command.Parameters.Add("@ObjectKey", MySqlDbType.String).Value = objectKey;
                    // Execute
                    await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        if (!await reader.ReadAsync(cancellationToken))
                            return null;
                        return StoredFile.Create(
                            id: reader.GetGuid("Id"),
                            originalName: reader.GetString("OriginalName"),
                            category: (StoredFileCategory)reader.GetByte("Category"),
                            mimeType: reader.GetString("MimeType"),
                            extension: reader.GetString("Extension"),
                            sizeInBytes: reader.GetInt64("SizeInBytes"),
                            objectKey: reader.GetString("ObjectKey"),
                            contentHash: reader.GetString("ContentHash"),
                            createdAt: new DateTimeOffset(reader.GetDateTime("CreatedAt"), TimeSpan.Zero)
                        );
                    }
                }
            }
        }
    }
}
