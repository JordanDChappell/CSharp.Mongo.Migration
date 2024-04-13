using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Interfaces;

/// <summary>
/// Migration locator contract, provides methods to retrieve migrations from different contexts.
/// </summary>
public interface IMigrationLocator {
    /// <summary>
    /// Retrieve a collection of `IMigration` instances that have not been run.
    /// </summary>
    /// <param name="collection">Collection of `MigrationDocument` database entries or migrations that have been 
    /// applied.</param>
    /// <returns>Collection of `IMigration` instances to run.</returns>
    public IEnumerable<IMigration> GetAvailableMigrations(IMongoCollection<MigrationDocument> collection);
    /// <summary>
    /// Retrieve a single `IMigration` instance by unique version identifier.
    /// </summary>
    /// <param name="version">Unique version identifier, corresponds to the `IMigration.Version` property.</param>
    /// <returns>`IMigration` instance if found, else null.</returns>
    public IMigration? GetMigration(string version);
}
