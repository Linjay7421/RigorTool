# RigorTools Infrastructure Integration Test Guidelines

Infrastructure integration tests prove concrete infrastructure implementations against real boundaries such as MySQL, filesystem storage, or provider implementations.

Use them for repositories, SQL, schema assumptions, migrations, connection factories, storage providers, and persistence behavior.

## Database Fixtures

Use database fixtures for shared and centralized control of connection factories.

Good pattern:

```csharp
private ProductDatabaseFixture _databaseFixture = default!;

[TestInitialize]
public void TestInitialize()
{
    _databaseFixture = new ProductDatabaseFixture();
}
```

Then pass the fixture connection factory into the concrete repository:

```csharp
var categoryRepository = new RawSqlCategoryRepository(_databaseFixture.ConnectionFactory);
```

Do not duplicate connection strings in individual tests when a fixture exists.

## Repository Tests

Repository tests should exercise the concrete repository implementation against the configured test database.

Useful assertions:

- expected rows are returned for meaningful filters
- SQL projections map expected columns
- null parent ids and hierarchy relationships are read correctly
- ordering and paging match the repository contract
- inserts, updates, and deletes persist expected values
- missing-data cases match the expected behavior

## Seed And Schema Assumptions

Use known seed data, CSV samples, or migration-provided rows for stable assertions. Keep the expected ids and counts close to the test or in a shared helper when reused.

When testing hierarchy or counts, distinguish repository responsibilities from handler responsibilities:

- repository integration tests prove SQL returns the expected flat rows/projections
- application unit tests prove tree-building edge cases from deterministic rows
- application integration tests prove the wired application flow returns expected seed-backed response structure

## Avoid

- Mocked repositories.
- MediatR pipeline assertions.
- Broad handler behavior that is already covered by application tests.
- Large brittle assertions over every seeded row unless the full export is the contract.
