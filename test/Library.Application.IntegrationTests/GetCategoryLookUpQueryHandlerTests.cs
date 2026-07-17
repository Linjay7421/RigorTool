using Library.Infrastructure.IntergrationTests.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Web.Library;
using Web.Library.Application.Abstractions;
using Web.Library.Application.Features.Category;
using Web.Library.Infrastructure.Persistence;
using Web.Library.Infrastructure.Repository.Common;

namespace Handlers.tests
{
    [TestClass]
    public sealed class GetCategoryLookUpQueryHandlerTests
    {
        private ServiceProvider _provider = null!;
        private CategoryTestHelper _categoryTestHelper = null!;
        private ProductDatabaseFixture _databaseFixture = null!;

        [TestInitialize]
        public void SetUp()
        {
            // Fixture
            _categoryTestHelper = new CategoryTestHelper();
            _databaseFixture = new ProductDatabaseFixture();
            
            // Service
            var services = new ServiceCollection();

            // Repository
            services.AddSingleton<IProductDbConnectionFactory>(_databaseFixture.ConnectionFactory);
            services.AddSingleton<ICategoryRepository, RawSqlCategoryRepository>();
            
            // Aplication layer
            services.AddApplication();
            services.AddLogging(builder =>
            {
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            _provider = services.BuildServiceProvider();
        }

        [TestMethod]
        [TestCategory("Intergration")]
        public async Task Handler_withoutGuid_shouldReturnAllRootCategoryLookups()
        {
            var mediator = _provider.GetRequiredService<IMediator>();

            var result = await mediator.Send(new GetCategoryLookUpQuery(null), CancellationToken.None);

            Assert.HasCount(_categoryTestHelper.RootCategoryCount, result);

            var root = result.Single(category => category.Id == _categoryTestHelper.HandToolsCategoryId);
            Assert.IsNull(root.ParentId);
            Assert.AreEqual(_categoryTestHelper.HandToolsCategoryName, root.Name);
            Assert.HasCount(_categoryTestHelper.HandToolsDirectChildCount, root.Children);

            CollectionAssert.AreEquivalent(
                _categoryTestHelper.HandToolsDirectChildIds.ToList(),
                root.Children.Select(category => category.Id).ToList());

            var extensionAndSlidingHandle = root.Children.Single(category => category.Id == _categoryTestHelper.ExtensionAndSlidingHandleCategoryId);
            Assert.AreEqual(_categoryTestHelper.HandToolsCategoryId, extensionAndSlidingHandle.ParentId);
            CollectionAssert.AreEquivalent(
                new List<Guid>
                {
                    _categoryTestHelper.ExtensionCategoryId,
                    _categoryTestHelper.SlidingHandleCategoryId,
                },
                extensionAndSlidingHandle.Children.Select(category => category.Id).ToList());
        }

        [TestMethod]
        [TestCategory("Intergration")]
        public async Task Handler_withGuid_shouldReturnRequestedCategoryLookupTree()
        {
            var mediator = _provider.GetRequiredService<IMediator>();

            var result = await mediator.Send(new GetCategoryLookUpQuery(_categoryTestHelper.HandToolsCategoryId), CancellationToken.None);

            Assert.HasCount(1, result);

            var handTools = result.Single();
            Assert.AreEqual(_categoryTestHelper.HandToolsCategoryId, handTools.Id);
            Assert.IsNull(handTools.ParentId);
            Assert.AreEqual(_categoryTestHelper.HandToolsCategoryName, handTools.Name);
            Assert.HasCount(_categoryTestHelper.HandToolsDirectChildCount, handTools.Children);

            CollectionAssert.AreEquivalent(
                _categoryTestHelper.HandToolsDirectChildIds.ToList(),
                handTools.Children.Select(category => category.Id).ToList());

            var extensionAndSlidingHandle = handTools.Children.Single(category => category.Id == _categoryTestHelper.ExtensionAndSlidingHandleCategoryId);
            Assert.AreEqual(_categoryTestHelper.ExtensionAndSlidingHandleCategoryName, extensionAndSlidingHandle.Name);
            Assert.HasCount(2, extensionAndSlidingHandle.Children);
        }
    }
}
