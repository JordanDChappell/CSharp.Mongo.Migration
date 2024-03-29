using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CSharp.Mongo.Migration.Models;

public class MigrationDocument {
    public ObjectId Id { get; set; }
    public string? Name { get; set; }
    public string? Version { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? AppliedUtc { get; set; }
}
