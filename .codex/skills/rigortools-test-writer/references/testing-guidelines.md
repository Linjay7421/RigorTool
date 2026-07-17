# RigorTools Testing Guidelines

Use the narrowest test type that proves the behavior under review. Keep application-layer tests and infrastructure tests separate because they answer different questions and have different setup costs.

Read the focused guide for the test type being written:

- `application-unit-test.md` for mocked handler, validator, behavior, and narrow MediatR resolution tests.
- `application-integration-test.md` for application DI, MediatR pipeline, and application tests that may use real fixture-backed repositories with seed-backed assertions.
- `infrastructure-integration-test.md` for repositories, SQL, schema assumptions, migrations, connection factories, and provider tests.

## Test Type Decision Guide

- Use application unit tests for handler business logic with mocked repositories.
- Use direct validator tests for validation rules.
- Use direct pipeline behavior tests for behavior-specific control flow.
- Use application integration tests for DI registration, handler discovery, pipeline wiring, validators, and handler flow against real fixture-backed repositories when requested.
- Use infrastructure integration tests for SQL, schema assumptions, connection factories, and repository implementations.

Avoid moving a test to a broader category just because the production path normally uses MediatR or a database. Broader tests are useful when the wiring or external boundary is the behavior being tested.

## Application Unit vs Application Integration vs Infrastructure Integration

Application unit tests cover use-case behavior without real database access. Prefer mocked repositories, in-memory data, direct handler construction, direct validators, and direct behavior invocation.

Application integration tests cover application requests through DI and MediatR. They may use real fixture-backed repositories when the user wants to prove handler + repository + database behavior. Ask for CSV, seed data, or known samples before writing hard database-backed assertions.

Infrastructure integration tests cover implementations that depend on external boundaries. Use a real database when testing repository behavior, SQL, schema assumptions, migrations, or connection factory behavior.

Keep database-backed tests clearly named as integration tests. They should make narrow persistence/query assertions, not broad handler, UI, or pipeline assertions.
