using Microsoft.Extensions.DependencyInjection;
using Moq;
using Web.Library.Application.Abstractions;
using Web.Library.Application.Features.Category;

namespace Handlers.Tests
{
    [TestClass]
    public sealed class GetCategoryLookUpQueryHandlerTests
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
        public async Task Handler_withoutGuid_shouldReturnAllRootCategoryLookups()
        {
            // Arrange
            var handler = _provider.GetRequiredService<GetCategoryLookUpQueryHandler>();

            // Act
            var result = await handler.Handle(new GetCategoryLookUpQuery(null), CancellationToken.None);

            // Assert
            Assert.HasCount(2, result);

            var root = result.Single(category => category.Id == _categoryTestHelper.RootCategoryId);
            Assert.IsNull(root.ParentId);
            Assert.AreEqual("Root category", root.Name);
            Assert.HasCount(1, root.Children);

            var child = root.Children.Single();
            Assert.AreEqual(_categoryTestHelper.ChildCategoryId, child.Id);
            Assert.AreEqual(_categoryTestHelper.RootCategoryId, child.ParentId);
            Assert.AreEqual(_categoryTestHelper.GrandchildCategoryId, child.Children.Single().Id);

            var otherRoot = result.Single(category => category.Id == _categoryTestHelper.OtherRootCategoryId);
            Assert.IsEmpty(otherRoot.Children);

            _categoryRepository.Verify(x => x.GetLookupAsync(), Times.Once);
            _categoryRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public async Task Handler_withGuid_shouldReturnRequestedCategoryLookupTree()
        {
            // Arrange
            var handler = _provider.GetRequiredService<GetCategoryLookUpQueryHandler>();

            // Act
            var result = await handler.Handle(new GetCategoryLookUpQuery(_categoryTestHelper.ChildCategoryId), CancellationToken.None);

            // Assert
            Assert.HasCount(1, result);

            var child = result.Single();
            Assert.AreEqual(_categoryTestHelper.ChildCategoryId, child.Id);
            Assert.AreEqual(_categoryTestHelper.RootCategoryId, child.ParentId);
            Assert.AreEqual("Child category", child.Name);
            Assert.HasCount(1, child.Children);
            Assert.AreEqual(_categoryTestHelper.GrandchildCategoryId, child.Children.Single().Id);

            _categoryRepository.Verify(x => x.GetByIdAsync(_categoryTestHelper.ChildCategoryId), Times.Once);
            _categoryRepository.Verify(x => x.GetLookupAsync(), Times.Never);
        }
    }
}
