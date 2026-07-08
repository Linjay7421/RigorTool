using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Public.Common.Behaviors;
using Web.Public.Features.Category;
using Web.Public.Features.Category.Models;
using Web.Public.Features.Product;
using Web.Public.Repository;
using Web.Public.Repository.Common;

namespace Web.Public.Application.Tests
{
    [TestClass]
    public sealed class GetTreeQueryHandlerTests
    {
        private ServiceProvider _provider = null!;

        [TestInitialize]
        public void Setup()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IDbConnectionFactory>(
                new MySqlConnectionFactory("Server=localhost;Port=13306;Database=ProductDB;Uid=root;Pwd=MyStrongPass123!;"));
            services.AddScoped<IValidator<GetCategoryTreeQuery>, GetCategoryTreeQueryValidator>();
            services.AddScoped<ICategoryRepository, RawSqlCategoryRepository>();

            services.AddMediatR(cfg =>
            {
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.RegisterServicesFromAssemblyContaining<GetCategoryTreeQuery>();
            });

            services.AddLogging(builder =>
            {
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            _provider = services.BuildServiceProvider();
        }

        [TestMethod]
        public async Task GetCategoryTreeQueryHandler_ShouldReturnTree()
        {
            using var scope = _provider.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var result = await mediator.Send(
                new GetCategoryTreeQuery(null),
                CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Any());
        } // Intergation test.

        [TestMethod]
        public async Task Handle_ShouldReturnSubTree_WhenRootIdIsProvided()
        {
            // Arrange
            var rootId = Guid.Parse("10000000-0000-0000-0000-000000000001");
            var childId = Guid.Parse("10000000-0000-0000-0000-000000000002");

            var categories = new List<Category>
            {
                new() { Id = rootId, Name = "A", ParentId = null },
                new() { Id = childId, Name = "A1", ParentId = rootId },
                new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "A1-1", ParentId = childId },
                new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Name = "B", ParentId = null }
            };

            var categoryReader = new Mock<ICategoryRepository>();

            categoryReader
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(categories);

            var handler = new GetCategoryTreeQueryHandler(categoryReader.Object);

            // Act
            var result = await handler.Handle(
                new GetCategoryTreeQuery(null),
                CancellationToken.None);

            // Assert
            Assert.HasCount(2, result);
            Assert.AreEqual("A", result[0].Name);
            Assert.AreEqual("A1", result[0].Children[0].Name);
            Assert.AreEqual("A1-1", result[0].Children[0].Children[0].Name);
        }
    }
}
