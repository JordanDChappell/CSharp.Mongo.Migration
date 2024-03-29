using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Infrastructure;

public static class DatabaseConnectionFactory {
    private static readonly Dictionary<string, IMongoDatabase> _databases = new();

    public static IMongoDatabase GetDatabase(string connectionString) {
        if (_databases.ContainsKey(connectionString))
            return _databases[connectionString];

        MongoUrl mongoUrl = new(connectionString);
        MongoClient mongoClient = new(mongoUrl);
        IMongoDatabase database = mongoClient.GetDatabase(mongoUrl.DatabaseName);

        _databases[connectionString] = database;

        return database;
    }
}
