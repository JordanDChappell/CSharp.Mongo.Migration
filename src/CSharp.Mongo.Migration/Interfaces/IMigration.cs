using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Interfaces;

public interface IMigration : IBaseMigration {
    public void Up<T>(IMongoCollection<T> collection);
    public void Down<T>(IMongoCollection<T> collection);
}
