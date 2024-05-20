using CSharp.Mongo.Migration.Core.Locators;
using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;
using CSharp.Mongo.Migration.Test.Data;
using CSharp.Mongo.Migration.Test.Infrastructure;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Test.Core.Locators;

public class ProvidedMigrationLocatorTests : DatabaseTest, IDisposable {
    private readonly IMongoCollection<MigrationDocument> _migrationCollection;

    public ProvidedMigrationLocatorTests(DatabaseTestFixture fixture) : base(fixture) {
        _migrationCollection = fixture.Database.GetCollection<MigrationDocument>("_migrations");
    }

    [Fact]
    public void GetMigrations_GivenNoMigrationsInDatabase_ShouldReturnAllMigrations() {
        List<IMigrationBase> migrations = new() {
            new TestMigration1(),
            new TestMigration2(),
            new TestMigration3(),
        };

        ProvidedMigrationLocator sut = new(migrations);

        IEnumerable<IMigrationBase> result = sut.GetAvailableMigrations(_migrationCollection);

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void GetMigrations_GivenMigrationsInDatabase_ShouldReturnUnappliedMigrations() {
        List<IMigrationBase> migrations = new() {
            new TestMigration1(),
            new TestMigration2(),
            new TestMigration3(),
        };

        _migrationCollection.InsertOne(new() {
            Name = migrations.First().Name,
            Version = migrations.First().Version,
        });

        ProvidedMigrationLocator sut = new(migrations);

        IEnumerable<IMigrationBase> result = sut.GetAvailableMigrations(_migrationCollection);

        Assert.Equal(2, result.Count());
    }

    public void Dispose() {
        _migrationCollection.DeleteMany(_ => true);
    }
}
