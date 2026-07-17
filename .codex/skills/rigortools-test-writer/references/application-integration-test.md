# RigorTools Application Integration Test Guidelines

Application integration tests prove that application requests work through application DI, MediatR, pipeline behaviors, validators, and the intended application boundary. In this project, `Library.Application.IntegrationTests` may use real fixture-backed repositories when the scenario is handler + repository + database behavior.

Do not assume application integration tests should always mock repositories. Follow the nearby test pattern and the user's intent.

## When To Use

Use application integration tests when proving:

- `IMediator.Send(...)` resolves the request handler.
- logging and validation behaviors are wired and can run.
- validators participate in the pipeline.
- a handler works with a real fixture-backed repository and seeded database rows.
- application output matches known seed data, CSV samples, or real test samples.

Avoid:

- exhaustive handler edge cases that are better covered by mocked unit tests.
- repository SQL-only assertions that belong in infrastructure integration tests.
- relying on unknown database contents.

## Seed-Backed Assertions

Before writing hard assertions against database-backed results, ask the user for a CSV, seed script, known ids, or a real test sample when it is not already present in the repo.

Use the sample to choose stable values:

- known ids
- expected names
- expected root counts
- expected child ids
- expected product counts when the repository owns that projection

Prefer structural assertions over row-order assumptions unless ordering is part of the contract.

## Shared Helpers

Put reusable assertion data in a shared test helper.

Good helper contents:

- known seeded ids
- expected names
- expected counts
- expected child-id collections
- small helper methods for common assertions

Avoid putting fake repository implementations in database-backed application integration helpers. Use the real repository with the fixture-provided connection factory.

## DI Pattern

Use application registration plus the real test fixture boundary when the integration test is database-backed:

```csharp
var services = new ServiceCollection();

services.AddSingleton<IProductDbConnectionFactory>(_databaseFixture.ConnectionFactory);
services.AddSingleton<ICategoryRepository, RawSqlCategoryRepository>();

services.AddApplication();

services.AddLogging(builder =>
{
    builder.AddDebug();
    builder.SetMinimumLevel(LogLevel.Information);
});
```

`AddApplication()` should register MediatR, `LoggingBehavior<,>`, `ValidationBehavior<,>`, and validators. If a test builds DI manually instead of using `AddApplication()`, include those pipeline pieces explicitly.

## Test Shape

Use `[TestInitialize]` for shared fixture/helper/provider setup.

Resolve `IMediator` from the provider and send the request:

```csharp
var mediator = _provider.GetRequiredService<IMediator>();
var result = await mediator.Send(new GetCategoryTreeQuery(categoryId), CancellationToken.None);
```

Assert on the application response, not raw database rows. Keep repository-specific SQL and schema checks in infrastructure integration tests.
