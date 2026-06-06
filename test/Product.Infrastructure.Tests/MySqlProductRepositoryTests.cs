using Web.Public.Repository;

namespace Product.Infrastructure.Tests
{
    [TestClass]
    public sealed class MySqlProductRepositoryTests
    {
        private const string TestConnectionString =
        "Server=localhost;Port=13306;Database=ProductDB;Uid=root;Pwd=MyStrongPass123!;";
        private Guid TestProductId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        [TestMethod]
        public async Task GetAllProducts_ShouldReturnAllProducts()
        {
            var factory = new MySqlConnectionFactory(TestConnectionString);
            var repository = new RawSqlProductRepository(factory);

            var products = await repository.GetAllProductsAsync();

            Assert.IsTrue(products.Any());
        }

        [TestMethod]
        public async Task GetProductById_ShouldReturnProdcut() {
            var factory = new MySqlConnectionFactory(TestConnectionString);
            var repository = new RawSqlProductRepository(factory);

            var product = await repository.GetProductByIdAsync(TestProductId);

            Assert.IsNotNull(product);
            Assert.AreEqual(product.Id, TestProductId);
        }
    }
}
