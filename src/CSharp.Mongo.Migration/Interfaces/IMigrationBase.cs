using CSharp.Mongo.Migration.Models;

namespace CSharp.Mongo.Migration.Interfaces;

/// <summary>
/// Migration properties contract, provides the structure that is used to find and manage migrations.
/// </summary>
public interface IMigrationBase {
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
    /// Interface method to simplify mapping an `MigrationDocument` instance.
    /// </summary>
    /// <returns></returns>
    internal MigrationDocument ToDocument() => new() {
        Name = Name,
        Version = Version,
        AppliedUtc = DateTime.UtcNow,
    };
}
