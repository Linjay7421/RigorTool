using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Web.Public.Common.Behaviors;
using Web.Public.Components;
using Web.Public.Features.Category;
using Web.Public.Features.Product;
using Web.Public.Repository;
using Web.Public.Repository.Common;
using Web.Public.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Repository dependcy injection.
builder.Services.AddSingleton<IProductDbConnectionFactory>(_ =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Missing Default connection string.");

    return new ProductDbConnectionFactory(connectionString);
}); // Product database.

builder.Services.AddSingleton<IStorageDbConnectionFactory>(_ =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("Storage")
        ?? throw new InvalidOperationException("Missing Storage connection string.");

    return new StorageDbConnectionFactory(connectionString);
}); // File database.

builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection("FileStorage"));

builder.Services.AddScoped<ICategoryRepository, RawSqlCategoryRepository>();
builder.Services.AddScoped<IProductRepository, RawSqlProductRepository>();

// Flunent Validator
builder.Services.AddScoped<IValidator<GetPagedSummaryQuery>, GetPagedSummaryQueryValidator>();
builder.Services.AddScoped<IValidator<GetCategoryTreeQuery>, GetCategoryTreeQueryValidator>();

// MediatR
builder.Services.AddMediatR(cfg => {
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.RegisterServicesFromAssembly(typeof(GetCategoryTreeQueryHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetPagedSummaryQueryHandler).Assembly);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
