using CSharp.Mongo.Migration.Infrastructure;

using Mongo2Go;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Test.Infrastructure;

public class DatabaseTestFixture : IDisposable {
    private readonly MongoDbRunner _mongoDbRunner;

    public IMongoDatabase Database { get; set; }
    public string ConnectionString { get; private set; }

    public DatabaseTestFixture() {
        _mongoDbRunner = MongoDbRunner.Start();
        ConnectionString = $"{_mongoDbRunner.ConnectionString}_test";
        Database = DatabaseConnectionFactory.GetDatabase(ConnectionString);
    }

    public void Dispose() {
        _mongoDbRunner?.Dispose();
    }
}
