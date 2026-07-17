using Microsoft.Extensions.DependencyInjection;
using Moq;
using Web.Library.Application.Abstractions;
using Web.Library.Application.Features.Category;

namespace Handlers.Tests
{
    [TestClass]
    public sealed class GetCategoryTreeQueryHandlerTests
    {
        private ServiceProvider _provider = null!;
        private CategoryTestHelper _categoryTestHelper = null!;
        private Mock<ICategoryRepository> _categoryRepository = null!;

        [TestInitialize]
        public void SetUp()
        {
            _categoryTestHelper = new CategoryTestHelper();
            _categoryRepository = _categoryTestHelper.CreateCategoryRepositoryMock();
            _provider = _categoryTestHelper
                .CreateCategoryServices(_categoryRepository)
                .BuildServiceProvider();
        }

        [TestMethod]
        [TestCategory("Unit")]
        public async Task Handler_withoutGuid_shouldReturnAllRootCategoryTrees()
        {
            // Arrange
            var handler = _provider.GetRequiredService<GetCategoryTreeQueryHandler>();

            // Act
            var result = await handler.Handle(new GetCategoryTreeQuery(null), CancellationToken.None);

            // Assert
            Assert.HasCount(2, result);

            var root = result.Single(category => category.Id == _categoryTestHelper.RootCategoryId);
            Assert.IsNull(root.ParentId);
            Assert.IsTrue(root.IsActive);
            Assert.AreEqual(3, root.ProductCount);
            Assert.HasCount(1, root.Children);

            var child = root.Children.Single();
            Assert.AreEqual(_categoryTestHelper.ChildCategoryId, child.Id);
            Assert.AreEqual(_categoryTestHelper.RootCategoryId, child.ParentId);
            Assert.AreEqual(2, child.ProductCount);
            Assert.AreEqual(_categoryTestHelper.GrandchildCategoryId, child.Children.Single().Id);

            var otherRoot = result.Single(category => category.Id == _categoryTestHelper.OtherRootCategoryId);
            Assert.AreEqual(0, otherRoot.ProductCount);
            Assert.IsEmpty(otherRoot.Children);

            _categoryRepository.Verify(x => x.GetTreeAsync(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public async Task Handler_withGuid_shouldReturnRequestedCategoryTree()
        {
            // Arrange
            var handler = _provider.GetRequiredService<GetCategoryTreeQueryHandler>();

            // Act
            var result = await handler.Handle(new GetCategoryTreeQuery(_categoryTestHelper.ChildCategoryId), CancellationToken.None);

            // Assert
            Assert.HasCount(1, result);

            var child = result.Single();
            Assert.AreEqual(_categoryTestHelper.ChildCategoryId, child.Id);
            Assert.AreEqual(_categoryTestHelper.RootCategoryId, child.ParentId);
            Assert.AreEqual("Child category", child.Name);
            Assert.AreEqual(2, child.ProductCount);
            Assert.HasCount(1, child.Children);

            var grandchild = child.Children.Single();
            Assert.AreEqual(_categoryTestHelper.GrandchildCategoryId, grandchild.Id);
            Assert.AreEqual(_categoryTestHelper.ChildCategoryId, grandchild.ParentId);
            Assert.IsFalse(grandchild.IsActive);
            Assert.AreEqual(1, grandchild.ProductCount);

            _categoryRepository.Verify(x => x.GetTreeAsync(), Times.Once);
        }
    }
}
