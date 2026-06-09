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
            var prodcutReader = new RawSqlProductRepository(factory);

            var products = await prodcutReader.GetAllAsync();

            Assert.IsTrue(products.Any());
        }

        [TestMethod]
        public async Task GetProductById_ShouldReturnProdcut() {
            var factory = new MySqlConnectionFactory(TestConnectionString);
            var prodcutReader = new RawSqlProductRepository(factory);

            var product = await prodcutReader.GetByIdAsync(TestProductId);

            Assert.IsNotNull(product);
            Assert.AreEqual(product.Id, TestProductId);
        }

        [TestMethod]
        public async Task GetProductsByCategory_ShouldReturnProducts() 
        {
            var factory = new MySqlConnectionFactory(TestConnectionString);
            var prodcutReader = new RawSqlProductRepository(factory);
            
            var result = await prodcutReader.GetPagedAsync(1, 2);

            Assert.IsNotNull(result);
            Assert.IsGreaterThan(0, result.TotalCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.AreEqual(2, result.PageSize);
            Assert.IsGreaterThanOrEqualTo(result.Items.Count, 1);
        }
    }
}
