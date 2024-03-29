using CSharp.Mongo.Migration.Infrastructure;
using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Core;

public class MigrationRunner : IMigrationRunner {
    private readonly IMongoDatabase _database;
    private readonly MigrationManager _manager;

    public MigrationRunner(string connectionString) {
        _database = DatabaseConnectionFactory.GetDatabase(connectionString);
        _manager = new();
    }

    public MigrationResult Run() {
        var migrations = _manager.GetMigrations(_database);

        return new();
    }

    public MigrationResult Run(string version) {
        var migrations = _manager.GetMigrations(_database, version);

        return new();
    }
}
