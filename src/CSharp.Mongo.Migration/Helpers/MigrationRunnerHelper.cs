using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;

namespace CSharp.Mongo.Migration.Helpers;

/// <summary>
/// Common helper methods used by the `MigrationRunner` class.
/// </summary>
public static class MigrationRunnerHelper {
    /// <summary>
    /// Find all migrations that can be run, where their dependencies have been recorded.
    /// </summary>
    /// <param name="migrations">Collection of `IOrderedMigration` instances.</param>
    /// <param name="completedMigrations">Collection of recorded `MigrationDocument` instances.</param>
    /// <returns>A collection of `IMigration` instances that can be run based on dependencies.</returns>
    public static IEnumerable<IOrderedMigration> FindMigrationsWhereDependenciesAreMet(
        IEnumerable<IOrderedMigration> migrations,
        IEnumerable<MigrationDocument> completedMigrations
    ) {
        List<string?> versions = GetDocumentVersions(completedMigrations);
        return migrations.Where(m => m.DependsOn.All(v => versions.Contains(v)));
    }

    /// <summary>
    /// Determine if there are any migrations in the collection that have not been recorded.
    /// </summary>
    /// <param name="migrations">Collection of `IOrderedMigration` instances.</param>
    /// <param name="completedMigrations">Collection of recorded `MigrationDocument` instances.</param>
    /// <returns>True if there are any migrations in the collection that have not been recorded, else false.</returns>
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
