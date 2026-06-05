using MySqlConnector;

namespace Web.Public.Repository
{
    public interface IDbConnectionFactory
    {
        MySqlConnection CreateConnection();
    }
}
