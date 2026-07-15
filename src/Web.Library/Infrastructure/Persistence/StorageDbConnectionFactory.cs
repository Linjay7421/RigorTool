using Web.Library.Infrastructure.Repository.Common;

namespace Web.Library.Infrastructure.Persistence
{
    public sealed class StorageDbConnectionFactory : MySqlConnectionFactory, IStorageDbConnectionFactory
    {
        public StorageDbConnectionFactory(string connectionString)
            : base(connectionString)
        {
        }
    }
}
