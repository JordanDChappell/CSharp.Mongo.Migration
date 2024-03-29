using System.Reflection;

using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration;

public class AssemblyMigrationLocator : IMigrationLocator {
    private readonly Assembly _assembly;
    private readonly Type _migrationType = typeof(IMigration);

    AssemblyMigrationLocator(Assembly assembly) {
        _assembly = assembly;
    }

    public IEnumerable<IMigration> GetMigrations(IMongoCollection<MigrationDocument> collection) {
        IEnumerable<Type> types = _assembly.GetTypes().Where(IsMigration);
        return types.Select(t => (IMigration)Activator.CreateInstance(t));
    }

    private bool IsMigration(Type type) => _migrationType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract;
}
