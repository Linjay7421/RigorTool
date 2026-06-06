using Web.Public.Repository;

namespace Product.Infrastructure.Tests
{
    [TestClass]
    public sealed class MySqlProductRepositoryTests
    {
        private const string TestConnectionString =
        "Server=localhost;Port=13306;Database=ProductDB;Uid=root;Pwd=MyStrongPass123!;";

        [TestMethod]
        public async Task GetAllProducts_ShouldReturnAllProducts()
        {
            var factory = new MySqlConnectionFactory(TestConnectionString);
            var repository = new RawSqlProductRepository(factory);

            var products = await repository.GetAllProductsAsync();

            Assert.IsTrue(products.Any());
        }
    }
}
