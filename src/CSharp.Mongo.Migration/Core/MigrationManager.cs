using CSharp.Mongo.Migration.Interfaces;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Core;

public class MigrationManager {
    public IEnumerable<IMigration>? GetMigrations(IMongoDatabase database, string version = "") {
        return default;
    }
}
