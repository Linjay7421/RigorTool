---
name: rigortools-test-writer
description: Write or improve tests for the RigorTools .NET solution. Use when adding MSTest coverage for MediatR handlers, pipeline behaviors, repositories, Blazor/application services, database-backed integration tests, or test-only dependency injection in this project.
---

# RigorTools Test Writer

## Core Workflow

1. Inspect the target production code and existing nearby tests before editing.
2. Classify the test type before choosing dependencies:
   - Handler unit test: instantiate the handler directly and mock repositories.
   - MediatR integration test: resolve `IMediator` from a test `ServiceProvider`.
   - Repository integration test: use real database configuration only when the test is explicitly database-backed.
   - Behavior test: instantiate the pipeline behavior directly unless the goal is to verify full MediatR wiring.
3. Keep tests focused. Do not add database access to unit tests that can use mocks.
4. Prefer deterministic in-memory data for tree-building and mapping logic.
5. Verify with the narrowest useful `dotnet test` command.

## RigorTools Patterns

- Use MSTest attributes: `[TestClass]`, `[TestInitialize]`, and `[TestMethod]`.
- Use `Moq` for repository fakes.
- Use `Microsoft.Extensions.DependencyInjection` for test DI.
- Register MediatR behaviors only in tests that intentionally exercise the pipeline:

```csharp
services.AddMediatR(cfg =>
{
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.RegisterServicesFromAssemblyContaining<GetCategoryTreeQuery>();
});
```

- Add logging providers in test DI only when the test needs logging infrastructure:

```csharp
services.AddLogging(builder =>
{
    builder.AddDebug();
    builder.SetMinimumLevel(LogLevel.Information);
});
```

Do not assert on the Visual Studio Debug window. If logging behavior needs coverage, verify behavior through a mock/fake logger or by asserting the pipeline calls `next()`.

## References

Read `references/testing-guidelines.md` first when choosing the test type.

Then read the focused guide for the layer under test:

- `references/application-testing.md` for handlers, validators, pipeline behaviors, and MediatR wiring tests.
- `references/infrastructure-testing.md` for repositories, SQL, schema assumptions, connection factories, and database-backed integration tests.
