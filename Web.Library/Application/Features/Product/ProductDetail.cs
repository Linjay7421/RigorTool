namespace Web.Library.Application.Features.Product
{
    public sealed class ProductDetail
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string? Description { get; init; }
        public IReadOnlyList<string> Features { get; init; } = [];
    }
}
