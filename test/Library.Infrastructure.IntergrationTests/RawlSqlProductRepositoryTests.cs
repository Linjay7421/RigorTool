using Library.Infrastructure.IntergrationTests.Common;
using Web.Library.Infrastructure.Persistence;

namespace Persistence.Repository.Tests
{
    [TestClass]
    public sealed class RawSqlProductRepositoryTests
    {

        private ProductDatabaseFixture _databaseFixture = default!;
        private Guid TestProductId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private readonly Guid categoryId = Guid.Parse("10000000-0000-0000-0000-000000000002");

        [TestInitialize]
        public void TestInitialize()
        {
            _databaseFixture = new ProductDatabaseFixture();
        }

        [TestMethod]
        [TestCategory("Intergration")]
        public async Task GetAll_ShouldReturnAllProducts()
        {
            var prodcutReader = new RawSqlProductRepository(_databaseFixture.ConnectionFactory);

            var products = await prodcutReader.GetAllAsync();

            Assert.IsTrue(products.Any());
        }

        [TestMethod]
        [TestCategory("Intergration")]
        public async Task GetById_ShouldReturnProdcut() {
            var prodcutReader = new RawSqlProductRepository(_databaseFixture.ConnectionFactory);

            var product = await prodcutReader.GetByIdAsync(TestProductId);

            Assert.IsNotNull(product);
            Assert.AreEqual(product.Id, TestProductId);
        }

        [TestMethod]
        [TestCategory("Intergration")]
        public async Task GetPaged_ShouldReturnPagedDto() 
        {
            var prodcutReader = new RawSqlProductRepository(_databaseFixture.ConnectionFactory);
            var result = await prodcutReader.GetPagedAsync(1, 2);

            Assert.IsNotNull(result);
            Assert.IsGreaterThan(0, result.TotalCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.AreEqual(2, result.PageSize);
            Assert.IsGreaterThanOrEqualTo(result.Items.Count, 1);
        }

        [TestMethod]
        [TestCategory("Intergration")]
        public async Task GetPagedByCategory_ShouldReturnPagedDto()
        {
            var prodcutReader = new RawSqlProductRepository(_databaseFixture.ConnectionFactory);

            var result = await prodcutReader.GetPagedAsync(1, 2, categoryId);

            Assert.IsNotNull(result);
            Assert.IsGreaterThan(0, result.TotalCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.AreEqual(2, result.PageSize);
            Assert.IsGreaterThanOrEqualTo(result.Items.Count, 1);
        }

        [TestMethod]
        [TestCategory("Intergration")]
        public async Task GetPagedByKeyword_ShouldReturnPagedDto()
        {
            var prodcutReader = new RawSqlProductRepository(_databaseFixture.ConnectionFactory);

            var result = await prodcutReader.GetPagedAsync(1, 2, keyword: "test");

            Assert.IsNotNull(result);
            Assert.IsGreaterThan(0, result.TotalCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.AreEqual(2, result.PageSize);
            Assert.IsGreaterThanOrEqualTo(result.Items.Count, 1);
        }
    }
}
