using MySqlConnector;

namespace Web.Public.Repository.Common
{
    public interface IDbConnectionFactory
    {
        MySqlConnection CreateConnection();
    }
}
