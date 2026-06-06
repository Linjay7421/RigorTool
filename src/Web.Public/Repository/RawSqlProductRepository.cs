using MySqlConnector;
using Web.Public.Common;

namespace Web.Public.Repository
{
    public class RawSqlProductRepository : IProductRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RawSqlProductRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Product>> GetAllProductsAsync()
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

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string sql = @"SELECT [Id], [Name] FROM [Product] WHERE [Id] = @ProductId";

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

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string categoryId)
        {
            var products = new List<Product>();

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string sql = @"
                SELECT p.*
                FROM Products p
                INNER JOIN ProductCategories pc
                    ON p.Id = pc.ProductId
                WHERE pc.CategoryId = @CategoryId";

            await using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@CategoryId", categoryId);

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

        public async Task<PagedResult<Product>> GetProductsPagedAsync(int pageNumber, int pageSize)
        {
            int offset = pageSize * (pageNumber - 1);
            var products = new List<Product>();
            int totalCount = 0;

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string sql = @"
                    SELECT Id, Name
                    FROM Products
                    WHERE IsActive = 1
                    ORDER BY CreatedAt DESC
                    LIMIT @PageSize OFFSET @SkipRows;

                    SELECT COUNT(*)
                    FROM Products
                    WHERE IsActive = 1;
                ";

            await using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@SkipRows", offset);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new Product
                {
                    Id = reader.GetGuid("Id"),
                    Name = reader.GetString("Name")
                });
            }

            await reader.NextResultAsync();

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

        public Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            throw new NotImplementedException();
        }
    }
}
