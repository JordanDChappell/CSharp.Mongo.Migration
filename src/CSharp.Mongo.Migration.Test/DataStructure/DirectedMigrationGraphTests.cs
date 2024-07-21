﻿
using CSharp.Mongo.Migration.DataStructure;
using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Test.Data;

namespace CSharp.Mongo.Migration.Test.DataStructure;

public class DirectedMigrationGraphTests {
    [Fact]
    public void IsCyclic_GivenLinearDependencies_ShouldReturnFalse() {
        List<IMigrationBase> migrations = new() {
            new TestMigration1(),
            new TestMigration2(),
            new TestMigration3(),
            new TestMigration4(),
            new TestMigration5(),
        };

        DirectedMigrationGraph graph = new(migrations);

        Assert.False(graph.IsCyclic());
    }

    [Fact]
    public void IsCyclic_GivenCyclicalDependencies_ShouldReturnTrue() {
        List<IMigrationBase> migrations = new() {
            new TestCyclicMigration1(),
            new TestCyclicMigration2(),
        };

        DirectedMigrationGraph graph = new(migrations);

        Assert.True(graph.IsCyclic());
    }

    [Fact]
    public void IsValid_GivenValidDependencies_ShouldReturnFalse() {
        List<IMigrationBase> migrations = new() {
            new TestMigration1(),
            new TestMigration2(),
            new TestMigration3(),
            new TestMigration4(),
            new TestMigration5(),
        };

        DirectedMigrationGraph graph = new(migrations);

        Assert.False(graph.IsValid());
    }

    [Fact]
    public void IsValid_GivenInvalidDependencies_ShouldReturnTrue() {
        List<IMigrationBase> migrations = new() {
            new TestMigration1(),
            new TestMigration4(),
            new TestMigration5(),
            new TestInvalidMigration(),
        };

        DirectedMigrationGraph graph = new(migrations);

        Assert.True(graph.IsValid());
    }

    [Fact]
    public void GetOrderedMigrations_GivenInvalidGraph_ShouldThrowException() {
        List<IMigrationBase> migrations = new() {
            new TestMigration1(),
            new TestMigration4(),
            new TestMigration5(),
            new TestInvalidMigration(),
        };

        DirectedMigrationGraph graph = new(migrations);

        var exception = Assert.Throws<Exception>(graph.GetOrderedMigrations);
        Assert.Contains("Missing migration dependency", exception.Message);
    }

    [Fact]
    public void GetOrderedMigrations_GivenCyclicGraph_ShouldThrowException() {
        List<IMigrationBase> migrations = new() {
            new TestCyclicMigration1(),
            new TestCyclicMigration2(),
        };

        DirectedMigrationGraph graph = new(migrations);

        var exception = Assert.Throws<Exception>(graph.GetOrderedMigrations);
        Assert.Contains("Circular migration dependencies detected", exception.Message);
    }

    [Fact]
    public void GetOrderedMigrations_GivenValidAcyclicGraph_ShouldReturnMigrations() {
        List<IMigrationBase> migrations = new() {
            new TestMigration1(),
            new TestMigration2(),
            new TestMigration3(),
            new TestMigration4(),
            new TestMigration5(),
        };

        DirectedMigrationGraph graph = new(migrations);

        List<IMigrationBase> result = graph.GetOrderedMigrations();

        Assert.Equal(migrations.Count, result.Count);
        Assert.Equal(0, result.FindIndex(m => m.Version == migrations[0].Version));
        Assert.Equal(1, result.FindIndex(m => m.Version == migrations[3].Version));
        Assert.Equal(2, result.FindIndex(m => m.Version == migrations[4].Version));
        Assert.Equal(3, result.FindIndex(m => m.Version == migrations[1].Version));
        Assert.Equal(4, result.FindIndex(m => m.Version == migrations[2].Version));
    }
}
