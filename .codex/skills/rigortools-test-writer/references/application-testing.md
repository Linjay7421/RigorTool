# RigorTools Application Testing Guidelines

This file is retained for compatibility. Prefer the focused references:

- `application-unit-test.md` for mocked application-layer tests.
- `application-integration-test.md` for application DI/MediatR tests, including fixture-backed repository tests when requested.

Application tests cover use-case behavior without real database access. Prefer mocked repositories, deterministic in-memory data, direct handler construction, direct validators, and direct behavior invocation.

Use MediatR integration tests only when the scenario is about DI registration, request handler discovery, pipeline ordering, or end-to-end pipeline wiring.

## Handler Unit Tests

Use direct handler construction when testing business logic inside a handler. Mock repository interfaces with `Moq` and provide in-memory data.

Good fit:

- Tree building from flat category rows.
- Filtering by root category.
- Mapping repository results to response models.
- Empty data, missing parent, duplicate name, and ordering edge cases.

Avoid:

- Real database connections.
- MediatR pipeline behavior.
- FluentValidation pipeline behavior.
- Assertions about logging provider output.

## Validator Tests

Prefer direct validator tests for request and model validation rules. Instantiate the validator directly and assert valid or invalid results for focused inputs.

Good fit:

- Required fields.
- Length, range, and format rules.
- Cross-field rules.
- Conditional validation.
- Expected validation error property names or messages when they are part of the contract.

Avoid:

- `IMediator.Send(...)` unless the test is proving validation behavior is wired into the MediatR pipeline.
- Real repository or database access unless the validator itself intentionally depends on a boundary.
- Repeating every validator case through a pipeline integration test.

## Pipeline Behavior Tests

Prefer direct behavior tests for MediatR pipeline behaviors. Instantiate the behavior directly, provide the required dependencies, and pass a controlled `next()` delegate.

Useful assertions:

- `next()` is called exactly once when the behavior should continue.
- the response from `next()` is returned.
- exceptions are propagated or handled according to the behavior contract.
- logging, validation, transaction, or timing side effects are emitted when practical to verify.

Avoid using MediatR integration tests for every behavior edge case. Use MediatR only when the scenario is specifically about behavior registration, ordering, or end-to-end pipeline wiring.

## MediatR Integration Tests

Use `ServiceCollection` plus `IMediator.Send(...)` when the test is meant to prove DI registration, request handler discovery, or pipeline behavior wiring.

Include:

- `services.AddMediatR(...)`
- pipeline behaviors that are part of the scenario
- `services.AddLogging(...)` only if the resolved behavior requires `ILogger`
- mocked repositories unless the test is explicitly database-backed

These tests should prove that the request can travel through MediatR. They should not duplicate all handler, validator, or behavior edge cases.

## Logging Behavior Tests

Prefer direct behavior tests for `LoggingBehavior<TRequest,TResponse>`. MediatR integration coverage should be limited to proving the logging behavior is registered and can run in the pipeline when that wiring matters.

Useful assertions:

- `next()` is called exactly once.
- the response from `next()` is returned.
- logger receives expected information-level calls when logger verification is practical.

Avoid using `Console.WriteLine`, Visual Studio Output, or Debug-window visibility as the test oracle.
