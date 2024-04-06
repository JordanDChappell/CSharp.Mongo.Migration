using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

namespace CSharp.Mongo.Migration;

public static class MigrationRunnerHelper {
    public static IEnumerable<IOrderedMigration> FindMigrationsWhereDependenciesAreMet(
        IEnumerable<IOrderedMigration> migrations,
        IEnumerable<MigrationDocument> completedMigrations
    ) {
        List<string?> versions = GetDocumentVersions(completedMigrations);
        return migrations.Where(m => m.DependsOn.All(v => versions.Contains(v)));
    }

    public static bool AnyMigrationsLeftToRun(
        IEnumerable<IOrderedMigration> migrations,
        IEnumerable<MigrationDocument> completedMigrations
    ) {
        List<string> expectedVersions = GetMigrationVersions(migrations);
        List<string?> completedVersions = GetDocumentVersions(completedMigrations);
        return expectedVersions.Except(completedVersions).Any();
    }

    private static List<string> GetMigrationVersions(IEnumerable<IMigration> migrations) =>
        migrations.Select(m => m.Version).ToList();

    private static List<string?> GetDocumentVersions(IEnumerable<MigrationDocument> documents) =>
        documents.Select(m => m.Version).ToList();
}
