using CSharp.Mongo.Migration.Infrastructure;

using EphemeralMongo;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Test.Infrastructure;

public class DatabaseTestFixture {
    private readonly IMongoRunner _mongoDbRunner;

    public IMongoDatabase Database { get; set; }
    public string ConnectionString { get; private set; }

    public DatabaseTestFixture() {
        _mongoDbRunner = MongoRunner.Run();
        ConnectionString = $"{_mongoDbRunner.ConnectionString}/test";
        Database = DatabaseConnectionFactory.GetDatabase(ConnectionString);
    }
}
