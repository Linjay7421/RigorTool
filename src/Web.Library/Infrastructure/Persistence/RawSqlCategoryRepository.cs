using MySqlConnector;
using Web.Library.Application.Abstractions;
using Web.Library.Application.Features.Category;
using Web.Library.Infrastructure.Repository.Common;

namespace Web.Library.Infrastructure.Persistence
{
    public class RawSqlCategoryRepository : ICategoryRepository
    {
        private readonly IProductDbConnectionFactory _connectionFactory;

        public RawSqlCategoryRepository(IProductDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            await using var connection = _connectionFactory.CreateConnection();
                        await connection.OpenAsync();

            const string sql = @"
                SELECT EXISTS(SELECT 1 FROM ProductDB.Categories WHERE Id = @CategoryId) AS 'Exists';
            ";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@CategoryId", id.ToString());

            var result = await command.ExecuteScalarAsync();
            return Convert.ToBoolean(result);
        }

        public async Task<IReadOnlyList<Category>> GetLookupAsync()
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string sql = @"
                SELECT Id, ParentId, Name 
                FROM Categories
                ORDER BY ParentId, Name;
            ";

            using var command = new MySqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync();

            var categoriyList = new List<Category>();
            var ordinal = reader.GetOrdinal("ParentId");

            while (await reader.ReadAsync()) 
            {
                categoriyList.Add(new Category
                    {
                        Id = reader.GetGuid("Id"),
                        ParentId = reader.IsDBNull(ordinal) ? null : reader.GetGuid("ParentId"),
                        Name = reader.GetString("Name"),
                    }
                );
            }

            return categoriyList;
        }

        public async Task<IReadOnlyList<Category>> GetByIdAsync(Guid categoryId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string sql = @"
                WITH RECURSIVE CategoryTree AS (
                    -- Root category
                    SELECT Id, ParentId, Name
                    FROM Categories
                    WHERE Id = @CategoryId

                    UNION ALL

                    -- Children
                    SELECT c.Id, c.ParentId, c.Name
                    FROM Categories c
                    INNER JOIN CategoryTree ct
                        ON c.ParentId = ct.Id
                )
                SELECT *
                FROM CategoryTree;
            ";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@CategoryId", categoryId.ToString());

            await using var reader = await command.ExecuteReaderAsync();

            var categoriyList = new List<Category>();
            var ordinal = reader.GetOrdinal("ParentId");

            while (await reader.ReadAsync())
            {
                categoriyList.Add(new Category
                {
                    Id = reader.GetGuid("Id"),
                    ParentId = reader.IsDBNull(ordinal) ? null : reader.GetGuid("ParentId"),
                    Name = reader.GetString("Name"),
                }
                );
            }
            return categoriyList;
        }
    }
}
