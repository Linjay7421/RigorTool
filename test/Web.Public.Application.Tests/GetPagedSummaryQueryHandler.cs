using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Web.Public.Common;
using Web.Public.Features.Category.Models;
using Web.Public.Features.Product;
using Web.Public.Features.Product.Models;
using Web.Public.Repository;
using Web.Public.Repository.Common;

namespace Web.Public.Application.Tests;

[TestClass]
public class GetPagedSummaryQueryHandlerTests
{
    private ServiceProvider _provider = null!;

    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IDbConnectionFactory>(
                new MySqlConnectionFactory("Server=localhost;Port=13306;Database=ProductDB;Uid=root;Pwd=MyStrongPass123!;"));

        services.AddScoped<IProductRepository, RawSqlProductRepository>();
        services.AddScoped<ICategoryRepository, RawSqlCategoryRepository>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<GetPagedSummaryQuery>();
        });

        _provider = services.BuildServiceProvider();
    }

    [TestMethod]
    public async Task Handler_ShouldReturnPagedSummary()
    {
        // Arrange
        var categoryId = Guid.Parse("10000000-0000-0000-0000-000000000001");

        var products = new List<ProductSummary>
        {
            new() { Id = Guid.NewGuid(), Name = "Product 1", Price = 10.0m },
            new() { Id = Guid.NewGuid(), Name = "Product 2", Price = 20.0m },
        };

        var pagedResult = new PagedResult<ProductSummary>
        {
            Items = products,
            TotalCount = products.Count,
            PageNumber = 1,
            PageSize = 10
        };

        var categoryReader = new Mock<ICategoryRepository>();
        var productReader = new Mock<IProductRepository>();

        categoryReader
            .Setup(x => x.ExistsAsync(categoryId))
            .ReturnsAsync(true);

        productReader
            .Setup(x => x.GetPagedAsync(1, 10, categoryId, null))
            .ReturnsAsync(pagedResult);

        var handler = new GetPagedSummaryQueryHandler(
            productReader.Object,
            categoryReader.Object);

        // Act
        var result = await handler.Handle(
            new GetPagedSummaryQuery(1, 10, categoryId, null),
            CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Items.Count);
        Assert.AreEqual(2, result.TotalCount);
        Assert.AreEqual(1, result.PageNumber);
        Assert.AreEqual(10, result.PageSize);

        // Moq verifications
        categoryReader.Verify(
            x => x.ExistsAsync(categoryId),
            Times.Once); // Executed once. 

        productReader.Verify(
            x => x.GetPagedAsync(1, 10, categoryId, null),
            Times.Once); // Executed once.
    } // Unit test.
}
