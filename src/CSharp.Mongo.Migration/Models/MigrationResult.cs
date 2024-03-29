namespace CSharp.Mongo.Migration.Models;

public class MigrationResult {
    public IEnumerable<MigrationDocument>? Steps { get; set; }
}
