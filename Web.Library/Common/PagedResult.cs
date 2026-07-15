namespace Web.Library.Common
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];
        public int TotalCount { get; init; }
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public int TotalPages() {
            return (int)Math.Ceiling(TotalCount / (double)PageSize);
        } 
        
        public bool HasPreviousPage() {
            return PageNumber > 1;
        }

        public bool HasNextPage() {
            return PageNumber < TotalPages();
        }
    }
}