using Microsoft.VisualBasic;

namespace Web.Library.Application.Features.Product
{
    public class ProductSummary
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Sku { get; init; } = string.Empty;
        public decimal Price { get; init; } = decimal.Zero;
        public string? ShortDescription { get; init; }
    }
}
