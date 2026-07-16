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
        public async Task GetLookup_ShouldReturnAllCategories()
        {
            var categoryReader = new RawSqlCategoryRepository(_databaseFixture.ConnectionFactory);

            var categories = await categoryReader.GetLookupAsync();

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
        public async Task GetTree_ShouldReturnCategoriesWithProductCounts()
        {
            var categoryReader = new RawSqlCategoryRepository(_databaseFixture.ConnectionFactory);

            var categories = await categoryReader.GetTreeAsync();

            Assert.IsTrue(categories.Any());

            var rootCategory = categories.SingleOrDefault(category => category.Id == categoryId);

            Assert.IsNotNull(rootCategory);
            Assert.AreEqual(categoryId, rootCategory.Id);
            Assert.IsFalse(string.IsNullOrWhiteSpace(rootCategory.Name));
            Assert.IsNull(rootCategory.ParentId);
            Assert.IsGreaterThanOrEqualTo(0, rootCategory.ProductCount);
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
