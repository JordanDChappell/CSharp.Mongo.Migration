using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration;

public interface IMigrationLocator {
    public IEnumerable<IMigration> GetMigrations(IMongoCollection<MigrationDocument> collection);
}
