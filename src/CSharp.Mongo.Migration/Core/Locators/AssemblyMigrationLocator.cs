﻿using System.Reflection;

using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration;

public class AssemblyMigrationLocator : IMigrationLocator {
    private readonly Assembly _assembly;
    private readonly Type _migrationType = typeof(IMigration);

    public AssemblyMigrationLocator(Assembly assembly) {
        _assembly = assembly;
    }

    public IEnumerable<IMigration> GetAvailableMigrations(IMongoCollection<MigrationDocument> collection) {
        IEnumerable<MigrationDocument> documents = collection.Find(_ => true).ToList();
        return GetMigrationsFromAssembly().Where(m => !documents.Any(d => d.Version == m.Version));
    }

    public IMigration? GetMigration(string version) =>
        GetMigrationsFromAssembly().FirstOrDefault(m => m.Version == version);

    private bool IsMigration(Type type) => _migrationType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract;

    private IEnumerable<IMigration> GetMigrationsFromAssembly() =>
        _assembly.GetTypes().Where(IsMigration).Select(t => (IMigration)Activator.CreateInstance(t));
}
