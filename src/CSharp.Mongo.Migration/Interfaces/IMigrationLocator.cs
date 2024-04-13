using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Interfaces;

/// <summary>
/// Migration locator contract, provides methods to retrieve migrations from different contexts.
/// </summary>
public interface IMigrationLocator {
    /// <summary>
    /// Retrieve a collection of `IMigrationBase` instances that have not been run.
    /// </summary>
    /// <param name="collection">Collection of `MigrationDocument` database entries or migrations that have been 
    /// applied.</param>
    /// <returns>Collection of `IMigrationBase` instances to run.</returns>
    public IEnumerable<IMigrationBase> GetAvailableMigrations(IMongoCollection<MigrationDocument> collection);
    /// <summary>
    /// Retrieve a single `IMigrationBase` instance by unique version identifier.
    /// </summary>
    /// <param name="version">Unique version identifier, corresponds to the `IMigrationBase.Version` property.</param>
    /// <returns>`IMigrationBase` instance if found, else null.</returns>
    public IMigrationBase? GetMigration(string version);
}
