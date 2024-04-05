using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Interfaces;

/// <summary>
/// Migration script contract, provides the structure that the `MigrationRunner` requires to run and manage migrations.
/// </summary>
public interface IMigration {
    /// <summary>
    /// Human readable migration name / description.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Unique version.
    /// </summary>
    public string Version { get; }
    /// <summary>
    /// Flags that this migration can be skipped, implement this member with conditional logic for more powerful
    /// migration selection.
    /// </summary>
    public bool Skip { get { return false; } }

    /// <summary>
    /// Function run when a migration is applied.
    /// </summary>
    /// <param name="database">MongoDB driver `IMongoDatabase` instance.</param>
    /// <returns>Task representing asynchronous database operations.</returns>
    public Task UpAsync(IMongoDatabase database);
    /// <summary>
    /// Function run when a migration is reverted.
    /// </summary>
    /// <param name="database">MongoDB driver `IMongoDatabase` instance.</param>
    /// <returns>Task representing asynchronous database operations.</returns>
    public Task DownAsync(IMongoDatabase database);

    /// <summary>
    /// Interface method to simplify mapping an `MigrationDocument` instance.
    /// </summary>
    /// <returns></returns>
    internal MigrationDocument ToDocument() => new() {
        Name = Name,
        Version = Version,
        AppliedUtc = DateTime.UtcNow,
    };
}
