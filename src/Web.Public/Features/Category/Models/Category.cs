namespace Web.Public.Features.Category.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public String Name { get; set; }
    }
}
