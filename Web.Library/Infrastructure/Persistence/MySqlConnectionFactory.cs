using MySqlConnector;
using System.Data.Common;
using Web.Library.Infrastructure.Repository.Common;
namespace Web.Library.Infrastructure.Persistence
{
    public class MySqlConnectionFactory: 
        IDbConnectionFactory,
        IStorageDbConnectionFactory,
        IProductDbConnectionFactory
    {
        private readonly string _connectionString;

        public MySqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
