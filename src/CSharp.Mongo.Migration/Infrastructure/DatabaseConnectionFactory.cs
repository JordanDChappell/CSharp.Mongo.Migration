using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Infrastructure;

/// <summary>
/// Manage connections to MongoDB.
/// </summary>
public static class DatabaseConnectionFactory {
    private static readonly Dictionary<string, IMongoDatabase> _databases = new();

    /// <summary>
    /// Retrieve an instance of the MongoDB driver `IMongoDatabase` class with the provided connection.
    /// </summary>
    /// <param name="connectionString">MongoDB connection uri string.</param>
    /// <returns>New or, if already connected, existing database instance with provided uri.</returns>
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
