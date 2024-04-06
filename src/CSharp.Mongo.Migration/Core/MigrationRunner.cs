using CSharp.Mongo.Migration.Infrastructure;
using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Core;

/// <summary>
/// The core migration runner class, this is the public interface that is used to run and revert migrations.
/// </summary>
public class MigrationRunner : IMigrationRunner {
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<MigrationDocument> _migrationCollection;
    private IMigrationLocator? _migrationLocator;

    public string MigrationCollectionName { get; set; } = "_migrations";

    public MigrationRunner(string connectionString) {
        _database = DatabaseConnectionFactory.GetDatabase(connectionString);
        _migrationCollection = _database.GetCollection<MigrationDocument>(MigrationCollectionName);
    }

    public MigrationRunner(string connectionString, IMigrationLocator migrationLocator) {
        _database = DatabaseConnectionFactory.GetDatabase(connectionString);
        _migrationCollection = _database.GetCollection<MigrationDocument>(MigrationCollectionName);
        _migrationLocator = migrationLocator;
    }

    public IMigrationRunner RegisterLocator(IMigrationLocator locator) {
        _migrationLocator = locator;
        return this;
    }

    public async Task<MigrationResult> RunAsync() {
        if (_migrationLocator is null)
            throw MigrationLocatorException();

        IEnumerable<IMigration> migrations = _migrationLocator
            .GetAvailableMigrations(_migrationCollection)
            .Where(m => !m.Skip);

        return await RunMigrationsAsync(migrations);
    }

    public async Task<MigrationResult> RevertAsync(string version) {
        if (_migrationLocator is null)
            throw MigrationLocatorException();

        IMigration? migration = _migrationLocator.GetMigration(version);

        if (migration is null)
            throw new ArgumentException("Unable to locate migration with provided version", nameof(version));

        MigrationDocument? document = await _migrationCollection
            .Find(m => m.Version == version)
            .FirstOrDefaultAsync();

        if (document is null)
            throw new ArgumentException(
                "Migration with provided version has not been recorded in the database",
                nameof(version)
            );

        await migration.DownAsync(_database);
        await _migrationCollection.DeleteOneAsync(d => d.Version == version);

        return new() {
            Steps = new List<MigrationDocument>() { document },
        };
    }

    private async Task<MigrationDocument> RunMigrationAsync(IMigration migration) {
        await migration.UpAsync(_database);

        MigrationDocument document = migration.ToDocument();
        await _migrationCollection.InsertOneAsync(document);

        return document;
    }

    private async Task<MigrationResult> RunMigrationsAsync(IEnumerable<IMigration> migrations) {
        List<MigrationDocument> steps = new();

        // Separate regular from ordered migrations - run these in separate blocks
        IEnumerable<IOrderedMigration> orderedMigrations = migrations.OfType<IOrderedMigration>();
        IEnumerable<IMigration> basicMigrations = migrations.Except(orderedMigrations);

        foreach (IMigration migration in basicMigrations) {
            MigrationDocument document = await RunMigrationAsync(migration);
            steps.Add(document);
        }

        steps.AddRange(await RunOrderedMigrationsAsync(orderedMigrations));

        return new() {
            Steps = steps,
        };
    }

    private async Task<IEnumerable<MigrationDocument>> RunOrderedMigrationsAsync(
        IEnumerable<IOrderedMigration> migrations
    ) {
        List<MigrationDocument> steps = new();

        do {
            List<MigrationDocument> migrationDocuments = await _migrationCollection.Find(_ => true).ToListAsync();
            IEnumerable<IOrderedMigration> migrationsToRun = MigrationRunnerHelper.FindMigrationsWhereDependenciesAreMet(
                migrations,
                migrationDocuments
            );

            foreach (IMigration migration in migrationsToRun) {
                MigrationDocument document = await RunMigrationAsync(migration);
                steps.Add(document);
            }
        } while (MigrationRunnerHelper.AnyMigrationsLeftToRun(migrations, steps));

        return steps;
    }

    private ArgumentNullException MigrationLocatorException() => new(
        nameof(_migrationLocator),
        "Unable to run migrations without registering an `IMigrationLocator`"
    );
}
