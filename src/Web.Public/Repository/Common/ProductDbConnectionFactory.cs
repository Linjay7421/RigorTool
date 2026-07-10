namespace Web.Public.Repository.Common
{
    public sealed class ProductDbConnectionFactory : MySqlConnectionFactory, IProductDbConnectionFactory
    {
        public ProductDbConnectionFactory(string connectionString)
            : base(connectionString)
        {
        }
    }
}
