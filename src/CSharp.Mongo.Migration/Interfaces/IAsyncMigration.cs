using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Interfaces;

public interface IAsyncMigration {
    public Task UpAsync<T>(IMongoDatabase database);
    public Task DownAsync<T>(IMongoDatabase database);
}
