using CSharp.Mongo.Migration.Helpers;
using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Core.Locators;

/// <summary>
/// Direct provided migration locator, accepts a collection of `IMigrationBase` instances.
/// </summary>
public class ProvidedMigrationLocator : IMigrationLocator {
    private readonly IEnumerable<IMigrationBase> _migrations;

    public ProvidedMigrationLocator(IEnumerable<IMigrationBase> migrations) {
        _migrations = migrations;
    }

    public IEnumerable<IMigrationBase> GetAllMigrations() => _migrations;

    public IEnumerable<IMigrationBase> GetAvailableMigrations(IMongoCollection<MigrationDocument> collection) =>
        MigrationLocatorHelper.FilterCompletedMigrations(collection, _migrations);

    public IMigrationBase? GetMigration(string version) => _migrations.FirstOrDefault(m => m.Version == version);
}
