namespace Web.Public.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

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