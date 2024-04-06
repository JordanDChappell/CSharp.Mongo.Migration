using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Models;
using CSharp.Mongo.Migration.Test.Data;

namespace CSharp.Mongo.Migration.Test.Helpers;

public class MigrationRunnerHelperTests {
    [Fact]
    public void FindMigrationsWhereDependenciesAreMet_GivenAllDependenciesMet_ShouldReturnAll() {
        List<IOrderedMigration> orderedMigrations = new() {
            new TestMigration4(),
            new TestMigration5(),
        };
        List<MigrationDocument> migrationDocuments = new() {
            new MigrationDocument() {
                Version = "1993.10.05 migration1",
            },
            new MigrationDocument() {
                Version = "2024.01.01 migration4",
            },
        };

        var result = MigrationRunnerHelper.FindMigrationsWhereDependenciesAreMet(orderedMigrations, migrationDocuments);

        Assert.Equal(orderedMigrations.Count, result.Count());
    }

    [Fact]
    public void FindMigrationsWhereDependenciesAreMet_GivenNotAllDependenciesMet_ShouldReturnExpectedResult() {
        List<IOrderedMigration> orderedMigrations = new() {
            new TestMigration4(),
            new TestMigration5(),
        };
        List<MigrationDocument> migrationDocuments = new() {
            new MigrationDocument() {
                Version = "1993.10.05 migration1",
            },
        };

        var result = MigrationRunnerHelper.FindMigrationsWhereDependenciesAreMet(orderedMigrations, migrationDocuments);

        Assert.Single(result);
    }

    [Fact]
    public void AnyMigrationsLeftToRun_GivenNotAllDocumentsExist_ShouldReturnTrue() {
        List<IOrderedMigration> orderedMigrations = new() {
            new TestMigration4(),
            new TestMigration5(),
        };
        List<MigrationDocument> migrationDocuments = new() {
            new MigrationDocument() {
                Version = "2024.01.01 migration4",
            },
        };

        var result = MigrationRunnerHelper.AnyMigrationsLeftToRun(orderedMigrations, migrationDocuments);

        Assert.True(result);
    }

    [Fact]
    public void AnyMigrationsLeftToRun_GivenAllDocumentsExist_ShouldReturnFalse() {
        List<IOrderedMigration> orderedMigrations = new() {
            new TestMigration4(),
            new TestMigration5(),
        };
        List<MigrationDocument> migrationDocuments = new() {
            new MigrationDocument() {
                Version = "2024.01.01 migration4",
            },
            new MigrationDocument() {
                Version = "2024.02.01 migration5",
            },
        };

        var result = MigrationRunnerHelper.AnyMigrationsLeftToRun(orderedMigrations, migrationDocuments);

        Assert.False(result);
    }
}
