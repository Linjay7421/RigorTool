namespace Web.Public.Features.Category.Models
{
    public sealed class CategoryNode
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public Guid? ParentId { get; init; }
        public List<CategoryNode> Children { get; init; } = [];
    }
}
