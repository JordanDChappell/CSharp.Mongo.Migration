using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Helpers;

/// <summary>
/// Common helper methods used by `IMigrationLocator` instances.
/// </summary>
public static class MigrationLocatorHelper {
    /// <summary>
    /// Filter a collection of migrations to remove any that have already been recorded.
    /// </summary>
    /// <param name="collection">`MigrationDocument` database collection.</param>
    /// <param name="migrations">Collection of `IMigrationBase` instances.</param>
    /// <returns>Filtered collection of `IMigrationBase` instances.</returns>
    public static IEnumerable<IMigrationBase> FilterCompletedMigrations(
        IMongoCollection<MigrationDocument> collection,
        IEnumerable<IMigrationBase> migrations
    ) {
        IEnumerable<MigrationDocument> documents = collection.Find(_ => true).ToList();
        return migrations.Where(m => !documents.Any(d => d.Version == m.Version));
    }
}
