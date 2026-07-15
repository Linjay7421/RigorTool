using Library.Infrastructure.IntergrationTests.Common;
using Web.Library.Infrastructure.Persistence;

namespace Persistence.Repository.Tests
{
    [TestClass]
    public class RawSqlCategoryRepositoryTests
    {
        private ProductDatabaseFixture _databaseFixture = default!;
        private readonly Guid categoryId = Guid.Parse("10000000-0000-0000-0000-000000000001");

        [TestInitialize]
        [TestCategory("Intergration")]
        public void TestInitalize()
        {
            _databaseFixture = new ProductDatabaseFixture();
        } 


        [TestMethod]
        [TestCategory("Intergration")]
        public async Task GetAll_ShouldReturnAllCategories()
        {
            var categoryReader = new RawSqlCategoryRepository(_databaseFixture.ConnectionFactory);

            var categories = await categoryReader.GetAllAsync();

            Assert.IsTrue(categories.Any());
        }

        [TestMethod]
        [TestCategory("Intergration")]
        public async Task GetById_ShouldReturnCategoryChildren()
        {
            var categoryReader = new RawSqlCategoryRepository(_databaseFixture.ConnectionFactory);

            var categories = await categoryReader.GetByIdAsync(categoryId);

            Assert.IsTrue(categories.Any());
        }

        [TestMethod]
        [TestCategory("Intergration")]
        public async Task Exists_ShouldReturnTrueForExistingCategory()
        {
            var categoryReader = new RawSqlCategoryRepository(_databaseFixture.ConnectionFactory);
            var exists = await categoryReader.ExistsAsync(categoryId);
            Assert.IsTrue(exists);
        }
    }
}
