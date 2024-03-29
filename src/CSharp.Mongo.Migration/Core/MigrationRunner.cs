using CSharp.Mongo.Migration.Infrastructure;
using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Core;

public class MigrationRunner : IMigrationRunner {
    private readonly string migrationCollectionName = "_migrations";

    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<MigrationDocument> _migrationCollection;

    public MigrationRunner(string connectionString) {
        _database = DatabaseConnectionFactory.GetDatabase(connectionString);
        _migrationCollection = _database.GetCollection<MigrationDocument>(migrationCollectionName);
    }

    public async Task<MigrationResult> RunAsync() {
        IEnumerable<IMigration> migrations = MigrationManager.GetMigrations(_database);
        return await RunAsync(migrations);
    }

    public async Task<MigrationResult> RunAsync(string version) {
        IEnumerable<IMigration> migrations = MigrationManager.GetMigrations(_database, version);
        return await RunAsync(migrations);
    }

    private async Task<MigrationResult> RunAsync(IEnumerable<IMigration> migrations) {
        List<MigrationDocument> steps = [];

        foreach (IMigration migration in migrations) {
            await migration.UpAsync(_database);

            MigrationDocument document = migration.ToDocument();
            await _migrationCollection.InsertOneAsync(document);
            steps.Add(document);
        }

        return new() {
            Steps = steps,
        };
    }
}
