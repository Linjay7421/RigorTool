using Web.Public.Repository;
using Web.Public.Repository.Common;

namespace Cateogory.Infrastructure.Tests
{
    [TestClass]
    public class MySqlCategoryRepositoryTests
    {
        private const string testConnectionString =
            "Server=localhost;Port=13306;Database=ProductDB;Uid=root;Pwd=MyStrongPass123!;";

        private readonly Guid categoryId = Guid.Parse("10000000-0000-0000-0000-000000000001");

        [TestMethod]
        public async Task GetAll_ShouldReturnAllCategories()
        {
            var factory = new MySqlConnectionFactory(testConnectionString);
            var categoryReader = new RawSqlCategoryRepository(factory);

            var categories = await categoryReader.GetAllAsync();

            Assert.IsTrue(categories.Any());
        }

        [TestMethod]
        public async Task GetById_ShouldReturnCategoryChildren()
        {
            var factory = new MySqlConnectionFactory(testConnectionString);
            var categoryReader = new RawSqlCategoryRepository(factory);

            var categories = await categoryReader.GetByIdAsync(categoryId);

            Assert.IsTrue(categories.Any());
        }

        [TestMethod]
        public async Task Exists_ShouldReturnTrueForExistingCategory()
        {
            var factory = new MySqlConnectionFactory(testConnectionString);
            var categoryReader = new RawSqlCategoryRepository(factory);
            var exists = await categoryReader.ExistsAsync(categoryId);
            Assert.IsTrue(exists);
        }
    }
}
