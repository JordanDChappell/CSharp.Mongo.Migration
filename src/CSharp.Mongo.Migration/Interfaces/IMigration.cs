using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Interfaces;

public interface IMigration {
    public string Name { get; }
    public string Version { get; }
    public bool Skip { get { return false; } }

    public Task UpAsync(IMongoDatabase database);
    public Task DownAsync(IMongoDatabase database);

    public MigrationDocument ToDocument() => new() {
        Name = Name,
        Version = Version,
        AppliedUtc = DateTime.UtcNow,
    };
}
