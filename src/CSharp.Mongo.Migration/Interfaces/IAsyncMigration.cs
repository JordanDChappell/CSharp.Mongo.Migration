using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Interfaces;

/// <summary>
/// Asynchronous script contract, provides the structure that the `MigrationRunner` requires to run and manage 
/// migrations.
/// </summary>
public interface IAsyncMigration : IMigrationBase {
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
}
