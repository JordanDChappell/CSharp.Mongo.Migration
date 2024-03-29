namespace CSharp.Mongo.Migration.Models;

public class MigrationResult {
    public string? CurrentVersion { get; set; }
    public IEnumerable<Migration>? Steps { get; set; }
    public bool Success { get; set; }
}
