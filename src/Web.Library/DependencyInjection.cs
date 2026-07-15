using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Infrastructure.Providers;
using Web.Library.Application.Abstractions;
using Web.Library.Application.Behaviors;
using Web.Library.Application.Features.Category;
using Web.Library.Application.Features.Product;
using Web.Library.Application.Features.Uploader;
using Web.Library.Infrastructure.Persistence;
using Web.Library.Infrastructure.Repository.Common;
using Web.Library.Infrastructure.Storage;

namespace Web.Library
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            // Flunent Validator
            services.AddScoped<IValidator<GetPagedSummaryQuery>, GetPagedSummaryQueryValidator>();
            services.AddScoped<IValidator<GetCategoryTreeQuery>, GetCategoryTreeQueryValidator>();
            services.AddScoped<IValidator<UploadDocumentCommand>, UploadDocumentCommandValidator>();
            services.AddScoped<IValidator<UploadImageCommand>, UploadImageCommandValidator>();

            // MediatR
            services.AddMediatR(cfg =>
            {
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.RegisterServicesFromAssembly(typeof(GetCategoryTreeQueryHandler).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(GetPagedSummaryQueryHandler).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(UploadImageHandler).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(UploadDocumentHandler).Assembly);
            });

            return services;
        }

        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IProductRepository, RawSqlProductRepository>();
            services.AddScoped<ICategoryRepository, RawSqlCategoryRepository>();
            services.AddScoped<IStoredFileRepository, RawSqlStoredFileRepository>();
            services.AddScoped<IFileStorage, LocalFileStorage>();

            services.AddSingleton<IClock, SystemClock>();
            services.AddSingleton<IObjectKeyGenerator, DefaultObjectKeyGenerator>();


            // Product database.
            services.AddSingleton<IProductDbConnectionFactory>(_ =>
            {
                var connectionString =
                    configuration.GetConnectionString("Default")
                    ?? throw new InvalidOperationException("Missing Default connection string.");

                return new MySqlConnectionFactory(connectionString);
            });

            // File database.
            services.AddSingleton<IStorageDbConnectionFactory>(_ =>
            {
                var connectionString =
                    configuration.GetConnectionString("Storage")
                    ?? throw new InvalidOperationException("Missing Storage connection string.");

                return new MySqlConnectionFactory(connectionString);
            });

            services.Configure<FileStorageOptions>(
                configuration.GetSection("FileStorage"));

            services.AddScoped<ICategoryRepository, RawSqlCategoryRepository>();
            services.AddScoped<IProductRepository, RawSqlProductRepository>();

            return services;
        }
    }
}
