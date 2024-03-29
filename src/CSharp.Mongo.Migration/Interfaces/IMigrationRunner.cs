using CSharp.Mongo.Migration.Models;

namespace CSharp.Mongo.Migration.Interfaces;

public interface IMigrationRunner {
    public MigrationResult Run();
    public MigrationResult Run(string version);
}
