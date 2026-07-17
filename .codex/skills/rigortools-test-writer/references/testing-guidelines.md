# RigorTools Testing Guidelines

Use the narrowest test type that proves the behavior under review. Keep application-layer tests and infrastructure tests separate because they answer different questions and have different setup costs.

Read the focused guide for the layer being tested:

- `application-testing.md` for handlers, validators, pipeline behaviors, and MediatR wiring tests.
- `infrastructure-testing.md` for repositories, SQL, schema assumptions, migrations, connection factories, and database-backed integration tests.

## Test Type Decision Guide

- Use direct handler tests for handler business logic.
- Use direct validator tests for validation rules.
- Use direct pipeline behavior tests for behavior-specific control flow.
- Use MediatR integration tests for DI registration, handler discovery, and pipeline wiring.
- Use database integration tests for SQL, schema assumptions, connection factories, and repository implementations.

Avoid moving a test to a broader category just because the production path normally uses MediatR or a database. Broader tests are useful when the wiring or external boundary is the behavior being tested.

## Application vs Infrastructure Tests

Application tests cover use-case behavior without real database access. Prefer mocked repositories, in-memory data, direct handler construction, direct validators, and direct behavior invocation.

Infrastructure tests cover implementations that depend on external boundaries. Use a real database only when testing repository behavior, SQL, schema assumptions, migrations, or connection factory behavior.

Keep database-backed tests clearly named as integration tests. They should make narrow persistence/query assertions, not broad handler, UI, or pipeline assertions.
