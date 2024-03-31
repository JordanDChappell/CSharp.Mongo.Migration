using CSharp.Mongo.Migration.Models;

namespace CSharp.Mongo.Migration.Interfaces;

public interface IMigrationRunner {
    public IMigrationRunner RegisterLocator(IMigrationLocator locator);
    public Task<MigrationResult> RunAsync();
    public Task<MigrationResult> RestoreAsync(string version);
}
