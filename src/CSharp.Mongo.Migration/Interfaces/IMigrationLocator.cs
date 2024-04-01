using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration;

public interface IMigrationLocator {
    public IEnumerable<IMigration> GetAvailableMigrations(IMongoCollection<MigrationDocument> collection);
    public IMigration? GetMigration(string version);
}
