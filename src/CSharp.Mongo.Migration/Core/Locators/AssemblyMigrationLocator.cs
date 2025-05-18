using System.Reflection;

using CSharp.Mongo.Migration.Helpers;
using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Core.Locators;

/// <summary>
/// Locate all concrete classes that implement the `IMigrationBase` interface in an assembly.
/// </summary>
public class AssemblyMigrationLocator : IMigrationLocator {
    private readonly Assembly _assembly;
    private readonly Type _migrationType = typeof(IMigrationBase);

    public AssemblyMigrationLocator(Assembly assembly) {
        _assembly = assembly;
    }

    public AssemblyMigrationLocator(string assemblyFile) {
        _assembly = Assembly.LoadFrom(assemblyFile);
    }

    public IEnumerable<IMigrationBase> GetAllMigrations() => GetMigrationsFromAssembly();

    public IEnumerable<IMigrationBase> GetAvailableMigrations(IMongoCollection<MigrationDocument> collection) {
        IEnumerable<IMigrationBase> migrations = GetMigrationsFromAssembly();
        return MigrationLocatorHelper.FilterCompletedMigrations(collection, migrations);
    }

    public IMigrationBase? GetMigration(string version) =>
        GetMigrationsFromAssembly().FirstOrDefault(m => m.Version == version);

    private bool IsMigration(Type type) => _migrationType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract;

    private bool HasIgnoreMigrationAttribute(Type type) => type.IsDefined(typeof(IgnoreMigrationAttribute), false);

    private IEnumerable<IMigrationBase> GetMigrationsFromAssembly() => _assembly
        .GetTypes()
        .Where(IsMigration)
        .Where(t => !HasIgnoreMigrationAttribute(t))
        .Select(t => (IMigrationBase)Activator.CreateInstance(t));
}
