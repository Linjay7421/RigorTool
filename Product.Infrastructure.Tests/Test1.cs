using Web.Public.Repository;

namespace Product.Infrastructure.Tests
{
    [TestClass]
    public sealed class Test1
    {
        private const string TestConnectionString =
        "Server=localhost;Port=13306;Database=ProductDB_Test;Uid=root;Pwd=MyStrongPass123!;";

        [TestMethod]
        public async Task GetAllProducts_ShouldReturnProducts()
        {
            var factory = new MySqlConnectionFactory(TestConnectionString);
            var repository = new RawSqlProductRepository(factory);

            var products = await repository.GetAllProductsAsync();

            Assert.IsTrue(products.Any());
        }
    }
}
