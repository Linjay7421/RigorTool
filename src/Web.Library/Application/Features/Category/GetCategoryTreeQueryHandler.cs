using MediatR;
using Web.Library.Application.Abstractions;

namespace Web.Library.Application.Features.Category
{
    public class GetCategoryTreeQueryHandler : IRequestHandler<GetCategoryTreeQuery, IReadOnlyList<CategoryNode>>
    {
        private readonly ICategoryRepository _categoryReader;

        public GetCategoryTreeQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryReader = categoryRepository;
        }

        public async Task<IReadOnlyList<CategoryNode>> Handle(GetCategoryTreeQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryReader.GetAllAsync();

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
                        Children = BuildChildren(c.Id)
                    })
                    .ToList();
            }

            var tree = categories
                .Where(c => c.ParentId is null)
                .Select(c => new CategoryNode
                {
                    Id = c.Id,
                    Name = c.Name,
                    Children = BuildChildren(c.Id)
                })
                .ToList();

            return tree;
        }
    }
}
