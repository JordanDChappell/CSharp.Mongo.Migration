using CSharp.Mongo.Migration.Interfaces;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Core;

public static class MigrationManager {
    public static IEnumerable<IMigration> GetMigrations(IMongoDatabase database, string version = "") {
        IEnumerable<IMigration> migrations = [];
        return migrations.Where(m => !m.Skip);
    }
}
