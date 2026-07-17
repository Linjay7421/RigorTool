using MediatR;
using Web.Library.Application.Abstractions;

namespace Web.Library.Application.Features.Category
{
    public class GetCategoryLookUpQueryHandler : IRequestHandler<GetCategoryLookUpQuery, IReadOnlyList<CategoryNode>>
    {
        private readonly ICategoryRepository _categoryReader;

        public GetCategoryLookUpQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryReader = categoryRepository;
        }

        public async Task<IReadOnlyList<CategoryNode>> Handle(GetCategoryLookUpQuery request, CancellationToken cancellationToken)
        {
            var categories = request.CategoryId is null
                ? await _categoryReader.GetLookupAsync()
                : await _categoryReader.GetByIdAsync(request.CategoryId.Value);

            ILookup<Guid?, Category> childrenLookup =
                categories
                    .Where(c => c.ParentId != null)
                    .ToLookup(c => c.ParentId);

            List<CategoryNode> BuildChildren(Guid? parentId)
            {
                return childrenLookup[parentId]
                    .Select(c => new CategoryNode
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ParentId = c.ParentId,
                        Children = BuildChildren(c.Id)
                    })
                    .ToList();
            }

            var rootCategories = request.CategoryId is null
                ? categories.Where(c => c.ParentId is null)
                : categories.Where(c => c.Id == request.CategoryId.Value);

            var tree = rootCategories
                .Select(c => new CategoryNode
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    Children = BuildChildren(c.Id)
                })
                .ToList();

            return tree;
        }
    }
}
