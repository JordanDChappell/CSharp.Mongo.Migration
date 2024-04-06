namespace CSharp.Mongo.Migration.Interfaces;

/// <summary>
/// Ordered migration script contract, uses `IMigration` as it's base.
/// </summary>
public interface IOrderedMigration : IMigration {
    /// <summary>
    /// A collection of migration versions that the current migration script depends on.
    /// </summary>
    public IEnumerable<string> DependsOn { get; }
}
