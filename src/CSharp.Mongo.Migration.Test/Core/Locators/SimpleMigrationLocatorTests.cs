using CSharp.Mongo.Migration.Core.Locators;
using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

using Moq;

namespace CSharp.Mongo.Migration.Test.Core.Locators;

public class SimpleMigrationLocatorTests {
    private readonly AutoMoqer _moqer = new AutoMoqer();
    private readonly Mock<IMongoCollection<MigrationDocument>> _mockCollection;

    public SimpleMigrationLocatorTests() {
        _mockCollection = _moqer.GetMock<IMongoCollection<MigrationDocument>>();
    }

    [Fact]
    public void GetMigrations_GivenNoMigrationsInDatabase_ShouldReturnAllMigrations() {
        List<IMigration> migrations = new() {

        };

        SimpleMigrationLocator sut = new(migrations);

        IEnumerable<IMigration> result = sut.GetMigrations(_mockCollection.Object);


    }
}
