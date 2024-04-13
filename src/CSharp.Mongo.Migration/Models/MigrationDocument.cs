using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CSharp.Mongo.Migration.Models;

/// <summary>
/// Database document used to keep track of which migrations have run.
/// </summary>
public class MigrationDocument {
    /// <summary>
    /// MongoDB `ObjectId` GUID.
    /// </summary>
    public ObjectId Id { get; set; }
    /// <summary>
    /// Human readable migration name.
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// Unique migration version string, this could be a simple SemVer number, or a date stamp, or an issue number.
    /// </summary>
    public string? Version { get; set; }
    /// <summary>
    /// When the migration was applied to the database.
    /// </summary>
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? AppliedUtc { get; set; }
}
