using Web.Library.Infrastructure.Repository.Common;

namespace Web.Library.Infrastructure.Persistence
{
    public sealed class ProductDbConnectionFactory : MySqlConnectionFactory, IProductDbConnectionFactory
    {
        public ProductDbConnectionFactory(string connectionString)
            : base(connectionString)
        {
        }
    }
}
