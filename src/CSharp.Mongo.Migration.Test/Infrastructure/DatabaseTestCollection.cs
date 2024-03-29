namespace CSharp.Mongo.Migration.Test.Infrastructure;

[CollectionDefinition(CollectionName)]
public class DatabaseTestCollection : ICollectionFixture<DatabaseTestFixture> {
    public const string CollectionName = "CSharp.Mongo.Migration.Test";
}
