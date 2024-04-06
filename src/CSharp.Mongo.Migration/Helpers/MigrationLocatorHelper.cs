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
    /// <param name="migrations">Collection of `IMigration` instances.</param>
    /// <returns>Filtered collection of `IMigration` instances.</returns>
    public static IEnumerable<IMigration> FilterCompletedMigrations(
        IMongoCollection<MigrationDocument> collection,
        IEnumerable<IMigration> migrations
    ) {
        IEnumerable<MigrationDocument> documents = collection.Find(_ => true).ToList();
        return migrations.Where(m => !documents.Any(d => d.Version == m.Version));
    }
}
