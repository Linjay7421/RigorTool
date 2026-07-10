using Web.Public.Repository;
using Web.Public.Repository.Common;

namespace Repository.Tests
{
    [TestClass]
    public sealed class RawProductRepositoryTests
    {
        private const string TestConnectionString =
        "Server=localhost;Port=13306;Database=ProductDB;Uid=root;Pwd=MyStrongPass123!;";
        private Guid TestProductId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private readonly Guid categoryId = Guid.Parse("10000000-0000-0000-0000-000000000002");

        [TestMethod]
        public async Task GetAll_ShouldReturnAllProducts()
        {
            var factory = new ProductDbConnectionFactory(TestConnectionString);
            var prodcutReader = new RawSqlProductRepository(factory);

            var products = await prodcutReader.GetAllAsync();

            Assert.IsTrue(products.Any());
        }

        [TestMethod]
        public async Task GetById_ShouldReturnProdcut() {
            var factory = new ProductDbConnectionFactory(TestConnectionString);
            var prodcutReader = new RawSqlProductRepository(factory);

            var product = await prodcutReader.GetByIdAsync(TestProductId);

            Assert.IsNotNull(product);
            Assert.AreEqual(product.Id, TestProductId);
        }

        [TestMethod]
        public async Task GetPaged_ShouldReturnPagedDto() 
        {
            var factory = new ProductDbConnectionFactory(TestConnectionString);
            var prodcutReader = new RawSqlProductRepository(factory);
            var result = await prodcutReader.GetPagedAsync(1, 2);

            Assert.IsNotNull(result);
            Assert.IsGreaterThan(0, result.TotalCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.AreEqual(2, result.PageSize);
            Assert.IsGreaterThanOrEqualTo(result.Items.Count, 1);
        }

        [TestMethod]
        public async Task GetPagedByCategory_ShouldReturnPagedDto()
        {
            var factory = new ProductDbConnectionFactory(TestConnectionString);
            var prodcutReader = new RawSqlProductRepository(factory);

            var result = await prodcutReader.GetPagedAsync(1, 2, categoryId);

            Assert.IsNotNull(result);
            Assert.IsGreaterThan(0, result.TotalCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.AreEqual(2, result.PageSize);
            Assert.IsGreaterThanOrEqualTo(result.Items.Count, 1);
        }

        [TestMethod]
        public async Task GetPagedByKeyword_ShouldReturnPagedDto()
        {
            var factory = new ProductDbConnectionFactory(TestConnectionString);
            var prodcutReader = new RawSqlProductRepository(factory);

            var result = await prodcutReader.GetPagedAsync(1, 2, keyword: "test");

            Assert.IsNotNull(result);
            Assert.IsGreaterThan(0, result.TotalCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.AreEqual(2, result.PageSize);
            Assert.IsGreaterThanOrEqualTo(result.Items.Count, 1);
        }
    }
}
