using MySqlConnector;

namespace Web.Library.Infrastructure.Repository.Common
{
    public interface IDbConnectionFactory
    {
        MySqlConnection CreateConnection();
    }
}
