using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Interfaces;

/// <summary>
/// Migration script contract, provides the structure that the `MigrationRunner` requires to run and manage migrations.
/// </summary>
public interface IMigration : IMigrationBase {
    /// <summary>
    /// Function run when a migration is applied.
    /// </summary>
    /// <param name="database">MongoDB driver `IMongoDatabase` instance.</param>
    public void Up(IMongoDatabase database);
    /// <summary>
    /// Function run when a migration is reverted.
    /// </summary>
    /// <param name="database">MongoDB driver `IMongoDatabase` instance.</param>
    public void Down(IMongoDatabase database);
}
