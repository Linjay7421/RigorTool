# RigorTools Infrastructure Testing Guidelines

This file is retained for compatibility. Prefer `infrastructure-integration-test.md` for current repository, SQL, schema, fixture, and provider guidance.

Infrastructure tests cover implementations that depend on external boundaries. Use them for repositories, SQL, schema assumptions, migrations, connection factories, and other persistence or provider integrations.

Keep infrastructure tests separate from application tests. They are usually slower, have more setup, and answer whether the concrete implementation works against the real boundary.

## Database Integration Tests

Use a real database only when testing SQL, schema assumptions, connection factories, or repository implementations.

Good fit:

- Repository query and persistence behavior.
- SQL projection and filtering behavior.
- Schema assumptions that application code depends on.
- Migration or seed-data assumptions.
- Connection factory configuration.

Avoid:

- Repeating handler business-rule edge cases.
- Testing MediatR pipeline behavior.
- Broad UI or application workflow assertions.
- Using a database when mocked repository data would prove the behavior.

Keep database-backed tests clearly named as integration tests. Prefer narrow assertions that confirm persistence/query behavior.

## Repository Tests

Repository tests should exercise the concrete repository implementation against the configured test database boundary.

Useful assertions:

- The repository returns expected rows for meaningful filters.
- Inserts, updates, and deletes persist expected values.
- Ordering and paging match the repository contract.
- Null, empty, and missing-data cases match the expected behavior.

Avoid asserting every mapping or tree-building edge case here if that logic belongs to an application handler. Keep repository tests focused on persistence and query behavior.
