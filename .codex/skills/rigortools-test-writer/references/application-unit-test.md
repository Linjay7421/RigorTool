# RigorTools Application Unit Test Guidelines

Application unit tests prove application-layer behavior without real database, filesystem, or network boundaries. Prefer direct handler construction for handler business logic, direct validator construction for validation rules, and direct behavior construction for pipeline behavior control flow.

Use MediatR only when the test is intentionally checking handler discovery, request resolution, or behavior pipeline wiring.

## Handler Unit Tests

Use `Moq` to simulate repositories and other application abstractions. Provide deterministic in-memory data that directly describes the behavior under test.

Good fit:

- Tree building from flat category rows.
- Filtering by category/root id.
- Mapping repository results to response models.
- Empty data, missing parent, duplicate name, and ordering edge cases.
- Verifying the handler calls the expected repository method for with-guid vs without-guid requests.

Avoid:

- Real database connections.
- Real file storage or external providers.
- Broad assertions that belong to application integration tests.

## Shared Helpers

Create a focused test helper when multiple tests use the same fake data or DI setup.

Helpers may contain:

- deterministic fake entities/rows
- `Mock<T>` setup methods
- shared `ServiceCollection` setup
- known ids, names, counts, and timestamps used by assertions

Keep helper data small enough to understand in the test.

## MediatR Unit Tests

When a unit test intentionally resolves through `IMediator`, include the expected pipeline dependencies:

```csharp
services.AddScoped<IValidator<GetCategoryTreeQuery>, GetCategoryTreeQueryValidator>();

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
```

Use mocked repositories in these tests unless the user explicitly asks for a database-backed integration test.

## Validator Tests

Instantiate validators directly for validation rules. Assert valid/invalid outcomes and important property names or messages when they are part of the contract.

Avoid sending every validator case through MediatR.

## Pipeline Behavior Tests

Instantiate pipeline behaviors directly for behavior-specific control flow. Provide controlled dependencies and a controlled `next()` delegate.

Useful assertions:

- `next()` is called exactly once.
- the response from `next()` is returned.
- validation exceptions or other expected exceptions are propagated.
- logging can be verified through a mock/fake logger when practical.
