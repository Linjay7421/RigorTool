using Web.Public.Repository;

namespace Product.Infrastructure.Tests
{
    [TestClass]
    public class MySqlCategoryRepositoryTests
    {
        private const string testConnectionString =
            "Server=localhost;Port=13306;Database=ProductDB;Uid=root;Pwd=MyStrongPass123!;";

        [TestMethod]
        public async Task GetAll_ShouldReturnAllCategories()
        {
            var factory = new MySqlConnectionFactory(testConnectionString);
            var categoryReader = new RawSqlCategoryRepository(factory);

            var categories = await categoryReader.GetAllAsync();

            Assert.IsTrue(categories.Any());
        }
    }
}
