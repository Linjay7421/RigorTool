namespace Handlers.tests
{
    internal sealed class CategoryTestHelper
    {
        public Guid ImpactSocketCategoryId { get; } = Guid.Parse("10000000-0000-0000-0000-000000000001");
        public Guid HandToolsCategoryId { get; } = Guid.Parse("10000000-0000-0000-0000-000000000006");
        public Guid BreakerBarCategoryId { get; } = Guid.Parse("10000000-0000-0000-0000-000000000007");
        public Guid ExtensionAndSlidingHandleCategoryId { get; } = Guid.Parse("10000000-0000-0000-0000-000000000008");
        public Guid ExtensionCategoryId { get; } = Guid.Parse("10000000-0000-0000-0000-000000000009");
        public Guid SlidingHandleCategoryId { get; } = Guid.Parse("10000000-0000-0000-0000-000000000010");

        public string HandToolsCategoryName { get; } = "Hand Tools and Accessories";
        public string ExtensionAndSlidingHandleCategoryName { get; } = "Extension and F & Sliding Handle";

        public int RootCategoryCount { get; } = 12;
        public int HandToolsDirectChildCount { get; } = 9;

        public IReadOnlyCollection<Guid> HandToolsDirectChildIds { get; } =
        [
            Guid.Parse("10000000-0000-0000-0000-000000000007"),
            Guid.Parse("10000000-0000-0000-0000-000000000008"),
            Guid.Parse("10000000-0000-0000-0000-000000000011"),
            Guid.Parse("10000000-0000-0000-0000-000000000012"),
            Guid.Parse("10000000-0000-0000-0000-000000000013"),
            Guid.Parse("10000000-0000-0000-0000-000000000014"),
            Guid.Parse("10000000-0000-0000-0000-000000000015"),
            Guid.Parse("10000000-0000-0000-0000-000000000016"),
            Guid.Parse("10000000-0000-0000-0000-000000000017"),
        ];
    }
}
