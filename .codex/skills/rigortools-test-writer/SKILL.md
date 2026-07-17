---
name: rigortools-test-writer
description: Write or improve tests for the RigorTools .NET solution. Use when adding MSTest coverage for MediatR handlers, pipeline behaviors, repositories, Blazor/application services, database-backed integration tests, or test-only dependency injection in this project.
---

# RigorTools Test Writer

## Core Workflow

1. Inspect the target production code and existing nearby tests before editing.
2. Classify the test type before choosing dependencies:
   - Application unit test: mock repositories with `Moq`; instantiate handlers directly unless the test is intentionally checking MediatR resolution or pipeline wiring.
   - Application integration test: use application DI and real fixture-backed repositories when the test is meant to prove handler + repository + database behavior; ask the user for CSV, seed data, or known sample rows before writing hard assertions.
   - Infrastructure integration test: use database fixtures for shared connection factory setup and exercise concrete repositories/providers.
   - Behavior test: instantiate the pipeline behavior directly unless the goal is to verify full MediatR wiring.
3. Keep tests focused. Do not add database access to unit tests that can use mocks.
4. Prefer deterministic in-memory data for application unit tests, and seed-backed expected values for database-backed integration tests.
5. Verify with the narrowest useful `dotnet test` command.

## RigorTools Patterns

- Use MSTest attributes: `[TestClass]`, `[TestInitialize]`, and `[TestMethod]`.
- Use `Moq` for repository fakes.
- Use `Microsoft.Extensions.DependencyInjection` for test DI.
- Register MediatR behaviors in tests that intentionally exercise MediatR resolution or application integration wiring:

```csharp
services.AddMediatR(cfg =>
{
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.RegisterServicesFromAssemblyContaining<GetCategoryTreeQuery>();
});
```

- Add validators and logging providers when validation/logging behaviors are part of the tested pipeline:

```csharp
services.AddScoped<IValidator<GetCategoryTreeQuery>, GetCategoryTreeQueryValidator>();

services.AddLogging(builder =>
{
    builder.AddDebug();
    builder.SetMinimumLevel(LogLevel.Information);
});
```

Do not assert on the Visual Studio Debug window. If logging behavior needs coverage, verify behavior through a mock/fake logger or by asserting the pipeline calls `next()`.

## References

Read `references/testing-guidelines.md` first when choosing the test type.

Then read the focused guide for the test type:

- `references/application-unit-test.md` for mocked handler, validator, behavior, and narrow MediatR resolution tests.
- `references/application-integration-test.md` for application DI tests that may use real fixture-backed infrastructure and seed-backed assertions.
- `references/infrastructure-integration-test.md` for repositories, SQL, schema assumptions, connection factories, and database-backed provider tests.
