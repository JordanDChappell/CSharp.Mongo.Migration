namespace CSharp.Mongo.Migration.Interfaces;

/// <summary>
/// Ordered migration script contract, uses `IMigrationBase` as it's base.
/// </summary>
public interface IOrderedMigration : IMigrationBase {
    /// <summary>
    /// A collection of migration versions that the current migration script depends on.
    /// </summary>
    public IEnumerable<string> DependsOn { get; }
}
