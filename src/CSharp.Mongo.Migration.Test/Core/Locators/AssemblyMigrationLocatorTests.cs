using System.Reflection;

using CSharp.Mongo.Migration.Core.Locators;
using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;
using CSharp.Mongo.Migration.Test.Data;
using CSharp.Mongo.Migration.Test.Infrastructure;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Test.Core.Locators;

public class AssemblyMigrationLocatorTests : DatabaseTest, IDisposable {
    private readonly IMongoCollection<MigrationDocument> _migrationCollection;

    public AssemblyMigrationLocatorTests(DatabaseTestFixture fixture) : base(fixture) {
        _migrationCollection = fixture.Database.GetCollection<MigrationDocument>("_migrations");
    }

    [Fact]
    public void GetMigrations_GivenAssemblyAndNoMigrationsInDatabase_ShouldReturnAllMigrations() {
        AssemblyMigrationLocator sut = new(Assembly.GetExecutingAssembly());

        IEnumerable<IMigrationBase> result = sut.GetAvailableMigrations(_migrationCollection);

        Assert.Equal(5, result.Count());
    }

    [Fact]
    public void GetMigrations_GivenAssemblyNameAndNoMigrationsInDatabase_ShouldReturnAllMigrations() {
        AssemblyMigrationLocator sut = new("CSharp.Mongo.Migration.Test.dll");

        IEnumerable<IMigrationBase> result = sut.GetAvailableMigrations(_migrationCollection);

        Assert.Equal(5, result.Count());
    }

    [Fact]
    public void GetMigrations_GivenAssemblyAndMigrationsInDatabase_ShouldReturnUnappliedMigrations() {
        TestMigration1 migration = new();
        _migrationCollection.InsertOne(new() {
            Name = migration.Name,
            Version = migration.Version,
        });

        AssemblyMigrationLocator sut = new(Assembly.GetExecutingAssembly());

        IEnumerable<IMigrationBase> result = sut.GetAvailableMigrations(_migrationCollection);

        Assert.Equal(4, result.Count());
    }

    public void Dispose() {
        _migrationCollection.DeleteMany(_ => true);
    }
}
