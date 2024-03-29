using CSharp.Mongo.Migration.Core;
using CSharp.Mongo.Migration.Core.Locators;
using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;
using CSharp.Mongo.Migration.Test.Infrastructure;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Test;

public class MigrationRunnerTests : DatabaseTest {
    IMongoCollection<MigrationDocument> _migrationCollection;

    public MigrationRunnerTests(DatabaseTestFixture fixture) : base(fixture) {
        _migrationCollection = fixture.Database.GetCollection<MigrationDocument>("_migrations");
    }

    [Fact]
    public async Task RunAsync_GivenNoMigrationLocatorInstance_ShouldThrowArgumentNullException() {
        MigrationRunner sut = new(_fixture.ConnectionString);
        await Assert.ThrowsAsync<ArgumentNullException>(sut.RunAsync);
    }

    [Fact]
    public async Task RunAsync_GivenMigrationLocatorConstructorRegistered_ShouldRun() {
        MigrationRunner sut = new(
            _fixture.ConnectionString,
            new SimpleMigrationLocator(new List<IMigration>())
        );
        var result = await sut.RunAsync();
        Assert.Empty(result.Steps);
    }

    [Fact]
    public async Task RunAsync_GivenMigrationLocatorMethodRegistered_ShouldRun() {
        MigrationRunner sut = new(_fixture.ConnectionString);
        var result = await sut
            .RegisterLocator(new SimpleMigrationLocator(new List<IMigration>()))
            .RunAsync();
        Assert.Empty(result.Steps);
    }
}
