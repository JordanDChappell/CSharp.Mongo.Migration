
using CSharp.Mongo.Migration.DataStructure;
using CSharp.Mongo.Migration.Interfaces;
using CSharp.Mongo.Migration.Test.Data;

namespace CSharp.Mongo.Migration.Test.DataStructure;

public class DirectedMigrationGraphTests {
    [Fact]
    public void IsCyclic_GivenLinearDependencies_ShouldReturnFalse() {
        List<IMigrationBase> migrations = [
            new TestMigration1(),
            new TestMigration2(),
            new TestMigration3(),
            new TestMigration4(),
            new TestMigration5(),
        ];

        DirectedMigrationGraph graph = new(migrations);

        Assert.False(graph.IsCyclic());
    }

    [Fact]
    public void IsCyclic_GivenCyclicalDependencies_ShouldReturnTrue() {
        List<IMigrationBase> migrations = [
            new TestCyclicMigration1(),
            new TestCyclicMigration2(),
        ];

        DirectedMigrationGraph graph = new(migrations);

        Assert.True(graph.IsCyclic());
    }

    [Fact]
    public void GetOrderedMigrations_GivenCyclicGraph_ShouldThrowException() {
        List<IMigrationBase> migrations = [
            new TestCyclicMigration1(),
            new TestCyclicMigration2(),
        ];

        DirectedMigrationGraph graph = new(migrations);

        var exception = Assert.Throws<Exception>(graph.GetOrderedMigrations);
        Assert.Contains("Circular migration dependencies detected", exception.Message);
    }

    [Fact]
    public void GetOrderedMigrations_GivenValidAcyclicGraph_ShouldReturnMigrations() {
        List<IMigrationBase> migrations = [
            new TestMigration5(),
            new TestMigration4(),
            new TestMigration1(),
            new TestMigration2(),
            new TestMigration3(),
        ];

        DirectedMigrationGraph graph = new(migrations);

        List<IMigrationBase> result = graph.GetOrderedMigrations();

        Assert.Equal(migrations.Count, result.Count);
        Assert.Equal(0, result.FindIndex(m => m.Version == "1993.10.05 migration1"));
        Assert.Equal(1, result.FindIndex(m => m.Version == "1993.10.07 migration2"));
        Assert.Equal(2, result.FindIndex(m => m.Version == "1993.10.09 migration3"));
        Assert.Equal(3, result.FindIndex(m => m.Version == "2024.01.01 migration4"));
        Assert.Equal(4, result.FindIndex(m => m.Version == "2024.02.01 migration5"));
    }
}
