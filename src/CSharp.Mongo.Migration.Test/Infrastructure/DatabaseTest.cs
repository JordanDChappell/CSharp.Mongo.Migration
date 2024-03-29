namespace CSharp.Mongo.Migration.Test.Infrastructure;

[Collection(DatabaseTestCollection.CollectionName)]
public abstract class DatabaseTest {
    protected readonly DatabaseTestFixture _fixture;

    protected DatabaseTest(DatabaseTestFixture fixture) {
        _fixture = fixture;
    }
}
