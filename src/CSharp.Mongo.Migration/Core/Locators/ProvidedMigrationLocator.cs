using CSharp.Mongo.Migration.Helpers;
using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Core.Locators;

/// <summary>
/// Direct provided migration locator, accepts a collection of `IMigration` instances.
/// </summary>
public class ProvidedMigrationLocator : IMigrationLocator {
    private readonly IEnumerable<IMigration> _migrations;

    public ProvidedMigrationLocator(IEnumerable<IMigration> migrations) {
        _migrations = migrations;
    }

    public IEnumerable<IMigration> GetAvailableMigrations(IMongoCollection<MigrationDocument> collection) =>
        MigrationLocatorHelper.FilterCompletedMigrations(collection, _migrations);

    public IMigration? GetMigration(string version) => _migrations.FirstOrDefault(m => m.Version == version);
}
