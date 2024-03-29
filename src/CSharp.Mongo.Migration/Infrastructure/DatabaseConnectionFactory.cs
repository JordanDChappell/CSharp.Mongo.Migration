using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Infrastructure;

public static class DatabaseConnectionFactory {
    public static IMongoDatabase GetDatabase(string connectionString) {
        MongoUrl mongoUrl = new(connectionString);
        MongoClient mongoClient = new(mongoUrl);
        return mongoClient.GetDatabase(mongoUrl.DatabaseName);
    }
}
