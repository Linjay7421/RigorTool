namespace Web.Public.Repository.Common
{
    public sealed class StorageDbConnectionFactory : MySqlConnectionFactory, IStorageDbConnectionFactory
    {
        public StorageDbConnectionFactory(string connectionString)
            : base(connectionString)
        {
        }
    }
}
