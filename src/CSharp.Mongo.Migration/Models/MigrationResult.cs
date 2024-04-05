namespace CSharp.Mongo.Migration.Models;

/// <summary>
/// Provides insights into what was completed when a migration is applied or reverted.
/// </summary>
public class MigrationResult {
    /// <summary>
    /// A collection of the migrations that were applied or reverted during operations.
    /// </summary>
    public IEnumerable<MigrationDocument> Steps { get; set; } = Enumerable.Empty<MigrationDocument>();
}
