using MediatR;
using Web.Library.Application.Abstractions;

namespace Web.Library.Application.Features.Category
{
    public class GetCategoryTreeQueryHandler : IRequestHandler<GetCategoryTreeQuery, IReadOnlyList<CategoryTreeNode>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoryTreeQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IReadOnlyList<CategoryTreeNode>> Handle(GetCategoryTreeQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetTreeAsync();
            var childrenLookup = categories.ToLookup(c => c.ParentId);

            CategoryTreeNode BuildNode(CategoryTreeRow category)
            {
                return new CategoryTreeNode
                {
                    Id = category.Id,
                    Name = category.Name,
                    ParentId = category.ParentId,
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt,
                    ProductCount = category.ProductCount,
                    Children = childrenLookup[category.Id]
                        .Select(BuildNode)
                        .ToList()
                };
            }

            var rootCategories = request.RootId is null
                ? childrenLookup[null]
                : categories.Where(c => c.Id == request.RootId.Value);

            return rootCategories
                .Select(BuildNode)
                .ToList();
        }
    }
}
