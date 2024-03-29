using CSharp.Mongo.Migration.Infrastructure;
using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Core;

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
            throw new ArgumentNullException(
                nameof(_migrationLocator),
                "Unable to run migrations without registering an `IMigrationLocator`"
            );

        IEnumerable<IMigration> migrations = _migrationLocator
            .GetMigrations(_migrationCollection)
            .Where(m => !m.Skip);
        return await RunMigrationsAsync(migrations);
    }

    private async Task<MigrationDocument> RunMigrationAsync(IMigration migration) {
        await migration.UpAsync(_database);

        MigrationDocument document = migration.ToDocument();
        await _migrationCollection.InsertOneAsync(document);

        return document;
    }

    private async Task<MigrationResult> RunMigrationsAsync(IEnumerable<IMigration> migrations) {
        List<MigrationDocument> steps = new();

        foreach (IMigration migration in migrations) {
            MigrationDocument document = await RunMigrationAsync(migration);
            steps.Add(document);
        }

        return new() {
            Steps = steps,
        };
    }
}
