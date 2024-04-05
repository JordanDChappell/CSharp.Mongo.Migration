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

    public IEnumerable<IMigration> GetAvailableMigrations(IMongoCollection<MigrationDocument> collection) {
        IEnumerable<MigrationDocument> documents = collection.Find(_ => true).ToList();
        return _migrations.Where(m => !documents.Any(d => d.Version == m.Version));
    }

    public IMigration? GetMigration(string version) => _migrations.FirstOrDefault(m => m.Version == version);
}
