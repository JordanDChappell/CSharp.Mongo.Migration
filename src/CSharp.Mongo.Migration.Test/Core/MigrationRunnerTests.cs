using CSharp.Mongo.Migration.Core;
using CSharp.Mongo.Migration.Core.Locators;
using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;
using CSharp.Mongo.Migration.Test.Infrastructure;

using MongoDB.Bson;
using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Test.Core;

public class MigrationRunnerTests : DatabaseTest, IDisposable {
    private readonly IMongoCollection<MigrationDocument> _migrationCollection;
    private readonly MigrationRunner _sut;

    public MigrationRunnerTests(DatabaseTestFixture fixture) : base(fixture) {
        _migrationCollection = _fixture.Database.GetCollection<MigrationDocument>("_migrations");
        _sut = new(_fixture.ConnectionString);
    }

    [Fact]
    public async Task RunAsync_GivenNoMigrationLocatorInstance_ShouldThrowArgumentNullException() {
        await Assert.ThrowsAsync<ArgumentNullException>(_sut.RunAsync);
    }

    [Fact]
    public async Task RunAsync_GivenMigrationLocatorConstructorRegistered_ShouldRun() {
        MigrationRunner sut = new(
            _fixture.ConnectionString,
            new ProvidedMigrationLocator(new List<IMigration>())
        );
        var result = await sut.RunAsync();
        Assert.Empty(result.Steps);
    }

    [Fact]
    public async Task RunAsync_GivenMigrationLocatorMethodRegistered_ShouldRun() {
        var result = await _sut
            .RegisterLocator(new ProvidedMigrationLocator(new List<IMigration>()))
            .RunAsync();
        Assert.Empty(result.Steps);
    }

    [Fact]
    public async Task RunAsync_GivenMigrationDoesNotExist_ShouldRunMigration() {
        // Insert documents to migrate
        List<TestDocumentV1> documentsToMigrate = new() {
            new() { Name = "Uncle Bob", Age = 5, },
            new() { Name = "Ichigo Kurusaki", Age = 16, },
            new() { Name = "Fireman Sam", Age = 34, },
            new() { Name = "Mickey Mouse", Age = 106, },
        };
        await _fixture.Database.GetCollection<TestDocumentV1>("Documents").InsertManyAsync(documentsToMigrate);

        IMigration migrationToRun = new TestMigration1();
        var result = await _sut
            .RegisterLocator(new ProvidedMigrationLocator(new List<IMigration>() { migrationToRun }))
            .RunAsync();

        Assert.Single(result.Steps);
        Assert.Equal(migrationToRun.Version, result.Steps.First().Version);

        var documents = await _fixture.Database.GetCollection<TestDocumentV2>("Documents")
            .Find(_ => true)
            .ToListAsync();

        Assert.Equal(4, documents.Count);
        Assert.True(documents.All(d => documentsToMigrate.Any(m => m.Name == d.FullName)));

        var migrations = await _migrationCollection.Find(_ => true).ToListAsync();

        Assert.Single(migrations);
        Assert.Equal(migrationToRun.Version, migrations.First().Version);
    }

    [Fact]
    public async Task RunAsync_GivenMigrationDoesExist_ShouldNotRunMigration() {
        // Insert documents to migrate
        List<TestDocumentV2> documentsToMigrate = new() {
            new() { FullName = "Uncle Bob", Age = 5, },
            new() { FullName = "Ichigo Kurusaki", Age = 16, },
            new() { FullName = "Fireman Sam", Age = 34, },
            new() { FullName = "Mickey Mouse", Age = 106, },
        };
        await _fixture.Database.GetCollection<TestDocumentV2>("Documents").InsertManyAsync(documentsToMigrate);

        IMigration migrationToRun = new TestMigration1();
        await _migrationCollection.InsertOneAsync(new() {
            Name = migrationToRun.Name,
            Version = migrationToRun.Version
        });

        var result = await _sut
            .RegisterLocator(new ProvidedMigrationLocator(new List<IMigration>() { migrationToRun }))
            .RunAsync();

        Assert.Empty(result.Steps);

        var documents = await _fixture.Database.GetCollection<TestDocumentV2>("Documents")
            .Find(_ => true)
            .ToListAsync();

        Assert.Equal(4, documents.Count);
        Assert.True(documents.All(d => documentsToMigrate.Any(m => m.FullName == d.FullName)));
    }

    [Fact]
    public async Task RunAsync_GivenMixOfExistingAndNewMigrations_ShouldRunExpectedMigrations() {
        // Insert documents to migrate
        List<TestDocumentV2> documentsToMigrate = new() {
            new() { FullName = "Uncle Bob", Age = 5, },
            new() { FullName = "Ichigo Kurusaki", Age = 16, },
            new() { FullName = "Fireman Sam", Age = 34, },
            new() { FullName = "Mickey Mouse", Age = 106, },
        };
        await _fixture.Database.GetCollection<TestDocumentV2>("Documents").InsertManyAsync(documentsToMigrate);

        IMigration firstMigration = new TestMigration1();
        IMigration secondMigration = new TestMigration2();
        await _migrationCollection.InsertOneAsync(new() {
            Name = firstMigration.Name,
            Version = firstMigration.Version
        });

        var result = await _sut
            .RegisterLocator(new ProvidedMigrationLocator(new List<IMigration>() { firstMigration, secondMigration }))
            .RunAsync();

        Assert.Single(result.Steps);

        var documents = await _fixture.Database.GetCollection<TestDocumentV3>("Documents")
            .Find(_ => true)
            .ToListAsync();

        Assert.Equal(4, documents.Count);
        Assert.True(documents.All(d => documentsToMigrate.Any(
            m => (m.FullName?.Contains(d.FirstName ?? "") ?? false) && (m.FullName?.Contains(d.LastName ?? "") ?? false)
        )));

        var migrations = await _migrationCollection.Find(_ => true).ToListAsync();

        Assert.Equal(2, migrations.Count);
        Assert.Equal(firstMigration.Version, migrations.First().Version);
        Assert.Equal(secondMigration.Version, migrations.Last().Version);
    }

    [Fact]
    public async Task RestoreAsync_GivenNoMigrationLocatorInstance_ShouldThrowArgumentNullException() {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.RevertAsync(""));
    }

    [Fact]
    public async Task RestoreAsync_GivenCannotFindMigration_ShouldThrowArgumentException() {
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _sut.RegisterLocator(new ProvidedMigrationLocator(new List<IMigration>())).RevertAsync("")
        );
        Assert.Contains("Unable to locate migration with provided version", exception.Message);
    }

    [Fact]
    public async Task RestoreAsync_GivenCannotFindDocument_ShouldThrowArgumentException() {
        List<IMigration> migrations = new() { new TestMigration1() };
        IMigrationLocator migrationLocator = new ProvidedMigrationLocator(migrations);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _sut.RegisterLocator(migrationLocator).RevertAsync(migrations.First().Version)
        );
        Assert.Contains(
            "Migration with provided version has not been recorded in the database",
            exception.Message
        );
    }

    [Fact]
    public async Task RestoreAsync_GivenVersionAndAllPrerequisites_ShouldRestore() {
        // Insert documents to restore
        List<TestDocumentV2> documentsToRestore = new() {
            new() { FullName = "Uncle Bob", Age = 5, },
            new() { FullName = "Ichigo Kurusaki", Age = 16, },
            new() { FullName = "Fireman Sam", Age = 34, },
            new() { FullName = "Mickey Mouse", Age = 106, },
        };
        await _fixture.Database.GetCollection<TestDocumentV2>("Documents").InsertManyAsync(documentsToRestore);

        // Prepare migration locator
        List<IMigration> migrations = new() { new TestMigration1() };
        IMigrationLocator migrationLocator = new ProvidedMigrationLocator(migrations);

        // Insert expected migration document
        await _migrationCollection.InsertOneAsync(new MigrationDocument() {
            Version = migrations.First().Version
        });

        var result = await _sut
            .RegisterLocator(migrationLocator)
            .RevertAsync(migrations.First().Version);

        // Ensure result
        Assert.Single(result.Steps);
        Assert.Equal(migrations.First().Version, result.Steps.First().Version);

        // Ensure documents were restored
        var documents = await _fixture.Database.GetCollection<TestDocumentV1>("Documents")
            .Find(_ => true)
            .ToListAsync();

        Assert.Equal(4, documents.Count);
        Assert.True(documents.All(d => documentsToRestore.Any(m => m.FullName == d.Name)));

        // Ensure migration was removed from the database
        var migrationDocuments = await _migrationCollection.Find(_ => true).ToListAsync();

        Assert.Empty(migrationDocuments);
    }

    public void Dispose() {
        _migrationCollection.DeleteMany(_ => true);
        _fixture.Database.GetCollection<BsonDocument>("Documents").DeleteMany(_ => true);
    }
}
