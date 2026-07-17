using Microsoft.Extensions.DependencyInjection;
using Moq;
using Web.Library.Application.Abstractions;
using Web.Library.Application.Features.Category;

namespace Handlers.Tests
{
    internal sealed class CategoryTestHelper
    {
        public Guid RootCategoryId { get; } = Guid.Parse("10000000-0000-0000-0000-000000000001");
        public Guid ChildCategoryId { get; } = Guid.Parse("10000000-0000-0000-0000-000000000002");
        public Guid GrandchildCategoryId { get; } = Guid.Parse("10000000-0000-0000-0000-000000000003");
        public Guid OtherRootCategoryId { get; } = Guid.Parse("10000000-0000-0000-0000-000000000004");

        public DateTime CreatedAt { get; } = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public ServiceCollection CreateCategoryServices(Mock<ICategoryRepository> categoryRepository)
        {
            var services = new ServiceCollection();

            services.AddSingleton(categoryRepository);
            services.AddSingleton<ICategoryRepository>(categoryRepository.Object);
            services.AddTransient<GetCategoryLookUpQueryHandler>();
            services.AddTransient<GetCategoryTreeQueryHandler>();

            return services;
        }

        public Mock<ICategoryRepository> CreateCategoryRepositoryMock()
        {
            var categoryRepository = new Mock<ICategoryRepository>();

            categoryRepository
                .Setup(x => x.GetLookupAsync())
                .ReturnsAsync(CreateCategories());

            categoryRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => CreateCategoryTree(id));

            categoryRepository
                .Setup(x => x.GetTreeAsync())
                .ReturnsAsync(CreateCategoryTreeRows());

            return categoryRepository;
        }

        private IReadOnlyList<Category> CreateCategoryTree(Guid id)
        {
            var categories = CreateCategories();
            var childrenLookup = categories.ToLookup(category => category.ParentId);

            IEnumerable<Category> SelectTree(Guid parentId)
            {
                foreach (var child in childrenLookup[parentId])
                {
                    yield return child;

                    foreach (var descendant in SelectTree(child.Id))
                    {
                        yield return descendant;
                    }
                }
            }

            var root = categories.SingleOrDefault(category => category.Id == id);

            return root is null
                ? []
                : new[] { root }.Concat(SelectTree(root.Id)).ToList();
        }

        private List<Category> CreateCategories()
        {
            return
            [
                new()
                {
                    Id = RootCategoryId,
                    Name = "Root category",
                    ParentId = null,
                },
                new()
                {
                    Id = ChildCategoryId,
                    Name = "Child category",
                    ParentId = RootCategoryId,
                },
                new()
                {
                    Id = GrandchildCategoryId,
                    Name = "Grandchild category",
                    ParentId = ChildCategoryId,
                },
                new()
                {
                    Id = OtherRootCategoryId,
                    Name = "Other root category",
                    ParentId = null,
                },
            ];
        }

        private IReadOnlyList<CategoryTreeRow> CreateCategoryTreeRows()
        {
            return
            [
                new()
                {
                    Id = RootCategoryId,
                    Name = "Root category",
                    ParentId = null,
                    IsActive = true,
                    CreatedAt = CreatedAt,
                    ProductCount = 3,
                },
                new()
                {
                    Id = ChildCategoryId,
                    Name = "Child category",
                    ParentId = RootCategoryId,
                    IsActive = true,
                    CreatedAt = CreatedAt.AddDays(1),
                    ProductCount = 2,
                },
                new()
                {
                    Id = GrandchildCategoryId,
                    Name = "Grandchild category",
                    ParentId = ChildCategoryId,
                    IsActive = false,
                    CreatedAt = CreatedAt.AddDays(2),
                    ProductCount = 1,
                },
                new()
                {
                    Id = OtherRootCategoryId,
                    Name = "Other root category",
                    ParentId = null,
                    IsActive = true,
                    CreatedAt = CreatedAt.AddDays(3),
                    ProductCount = 0,
                },
            ];
        }
    }
}
