using MySqlConnector;
using System.Runtime.InteropServices;
using Web.Public.Common;
using Web.Public.Repository.Common;

namespace Web.Public.Repository
{
    public class RawSqlProductRepository : IProductRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RawSqlProductRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var products = new List<Product>();

            await using var connection = _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            var sql = "SELECT * FROM Products";

            await using var command = new MySqlCommand(sql, connection);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new Product
                {
                    Id = reader.GetGuid("Id"),
                    Name = reader.GetString("Name")
                });
            }

            return products;
        }

        public async Task<Product?> GetByIdAsync(Guid productId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string sql = @"SELECT Id, Name FROM Products WHERE Id = @ProductId";

            await using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ProductId", productId);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Product
                {
                    Id = reader.GetGuid("Id"),
                    Name = reader.GetString("Name")
                };
            }

            return null;
        }

        public async Task<PagedResult<Product>> GetPagedAsync(int pageNumber, int pageSize, Guid? categoryId = null, string? keyword = null)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var offset = pageSize * (pageNumber - 1);
            var products = new List<Product>();

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            #region Assemble sql query
            var sql = @"
                    SELECT p.Id, p.Name
                    FROM Products p
                    INNER JOIN ProductCategories pc
                        ON p.Id = pc.ProductId
                    WHERE p.IsActive = 1
                        AND (@CategoryId IS NULL OR pc.CategoryId = @CategoryId)
                        AND (@Keyword IS NULL OR p.Name LIKE @Keyword OR p.Sku LIKE @Keyword)
                    ORDER BY p.CreatedAt DESC
                    LIMIT @PageSize OFFSET @Offset;

                    SELECT COUNT(*)
                    FROM Products p
                    INNER JOIN ProductCategories pc
                        ON p.Id = pc.ProductId
                    WHERE p.IsActive = 1
                        AND (@CategoryId IS NULL OR pc.CategoryId = @CategoryId)
                        AND (@Keyword IS NULL OR p.Name LIKE @Keyword OR p.Sku LIKE @Keyword)
                ";
            
            await using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@Offset", offset);
            command.Parameters.Add("@CategoryId", MySqlDbType.VarChar).Value =
                categoryId.HasValue ? categoryId.Value.ToString() : DBNull.Value;

            command.Parameters.Add("@Keyword", MySqlDbType.VarChar).Value =
                string.IsNullOrWhiteSpace(keyword) ? DBNull.Value : $"%{keyword.Trim()}%";
            #endregion

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new Product
                    {
                        Id = reader.GetGuid("Id"),
                        Name = reader.GetString("Name")
                    }
                );
            }

            await reader.NextResultAsync();

            int totalCount = 0;

            if (await reader.ReadAsync())
            {
                totalCount = reader.GetInt32(0);
            }

            return new PagedResult<Product>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
