using MySqlConnector;
namespace Web.Public.Repository
{
    public class MySqlConnectionFactory
    {
        private readonly string? _connectionStr;
        
        public MySqlConnectionFactory(IConfiguration config)
        {
            this._connectionStr = config.GetConnectionString("Default");
            if (String.IsNullOrEmpty(this._connectionStr))
            {
                throw new ArgumentNullException("Connection string can not be null");
            }
        }

        public MySqlConnection CreateConnection => new MySqlConnection(this._connectionStr);
    }
}
