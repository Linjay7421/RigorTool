using MySqlConnector;
using System.Data.Common;
namespace Web.Public.Repository.Common
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
