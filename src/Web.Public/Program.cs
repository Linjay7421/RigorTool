using MediatR;
using Web.Public.Common.Behaviors;
using Web.Public.Components;
using Web.Public.Features.Category;
using Web.Public.Features.Product;
using Web.Public.Repository;
using Web.Public.Repository.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Repository dependcy injection.
builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException();

    return new MySqlConnectionFactory(connectionString);
});

builder.Services.AddScoped<ICategoryRepository, RawSqlCategoryRepository>();
builder.Services.AddScoped<IProductRepository, RawSqlProductRepository>();

builder.Services.AddMediatR(cfg => {
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
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
