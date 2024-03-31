using CSharp.Mongo.Migration.Core.Locators;
using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;
using CSharp.Mongo.Migration.Test.Infrastructure;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Test.Core.Locators;

public class SimpleMigrationLocatorTests : DatabaseTest, IDisposable {
    private readonly IMongoCollection<MigrationDocument> _migrationCollection;

    public SimpleMigrationLocatorTests(DatabaseTestFixture fixture) : base(fixture) {
        _migrationCollection = fixture.Database.GetCollection<MigrationDocument>("_migrations");
    }

    [Fact]
    public void GetMigrations_GivenNoMigrationsInDatabase_ShouldReturnAllMigrations() {
        List<IMigration> migrations = new() {
            new TestMigration1(),
            new TestMigration2(),
            new TestMigration3(),
        };

        SimpleMigrationLocator sut = new(migrations);

        IEnumerable<IMigration> result = sut.GetMigrations(_migrationCollection);

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void GetMigrations_GivenMigrationsInDatabase_ShouldReturnUnappliedMigrations() {
        List<IMigration> migrations = new() {
            new TestMigration1(),
            new TestMigration2(),
            new TestMigration3(),
        };

        _migrationCollection.InsertOne(new() {
            Name = migrations.First().Name,
            Version = migrations.First().Version,
        });

        SimpleMigrationLocator sut = new(migrations);

        IEnumerable<IMigration> result = sut.GetMigrations(_migrationCollection);

        Assert.Equal(2, result.Count());
    }

    public void Dispose() {
        _migrationCollection.DeleteMany(_ => true);
    }
}
