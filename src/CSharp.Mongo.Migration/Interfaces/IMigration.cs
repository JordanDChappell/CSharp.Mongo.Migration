using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Interfaces;

public interface IMigration : IBaseMigration {
    public void Up<T>(IMongoDatabase database);
    public void Down<T>(IMongoDatabase database);
}
