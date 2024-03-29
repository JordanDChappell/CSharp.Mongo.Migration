using CSharp.Mongo.Migration.Models;

namespace CSharp.Mongo.Migration.Interfaces;

public interface IMigrationRunner {
    public Task<MigrationResult> RunAsync(string version = "");
}
