using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Interfaces;

public interface IAsyncMigration {
    public Task UpAsync<T>(IMongoCollection<T> collection);
    public Task DownAsync<T>(IMongoCollection<T> collection);
}
