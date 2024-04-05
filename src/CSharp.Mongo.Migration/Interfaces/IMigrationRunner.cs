using CSharp.Mongo.Migration.Models;

namespace CSharp.Mongo.Migration.Interfaces;

/// <summary>
/// Migration runner contract, provides methods that are used to run and revert migrations.
/// </summary>
public interface IMigrationRunner {
    /// <summary>
    /// Register an `IMigrationLocator` instance with the current runner instance.
    /// </summary>
    /// <param name="locator">`IMigrationLocator` instance.</param>
    /// <returns>The current `IMigrationRunner` instance, `this`.</returns>
    public IMigrationRunner RegisterLocator(IMigrationLocator locator);
    /// <summary>
    /// Run all available migrations.
    /// </summary>
    /// <returns>Task representing a `MigrationResult` that includes which migrations were run.</returns>
    public Task<MigrationResult> RunAsync();
    /// <summary>
    /// Revert a specific migration.
    /// </summary>
    /// <param name="version">Unique migration version identifier.</param>
    /// <returns>Task representing a `MigrationResult` that includes the migration that was reverted.</returns>
    public Task<MigrationResult> RevertAsync(string version);
}
