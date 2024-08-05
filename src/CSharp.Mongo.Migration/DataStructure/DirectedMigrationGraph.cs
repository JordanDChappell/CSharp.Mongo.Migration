using CSharp.Mongo.Migration.Interfaces;

namespace CSharp.Mongo.Migration.DataStructure;

/// <summary>
/// A directed graph used to contain and act upon ordered migrations.
/// </summary>
public class DirectedMigrationGraph {
    /// <summary>
    /// A reference to all migrations, not just those that exist in the dependency graph.
    /// </summary>
    private readonly IEnumerable<IMigrationBase> _allMigrations;
    /// <summary>
    /// Migrations with their dependencies as graph nodes.
    /// </summary>
    private readonly Dictionary<string, List<string>> _nodes = [];

    public DirectedMigrationGraph(IEnumerable<IMigrationBase> migrations) {
        _allMigrations = migrations;
        BuildGraph();
    }

    /// <summary>
    /// Build the `_nodes` dictionary from the collection of migrations.
    /// </summary>
    private void BuildGraph() {
        foreach (IMigrationBase migration in _allMigrations) {
            if (migration is IOrderedMigration orderedMigration)
                _nodes.Add(orderedMigration.Version, orderedMigration.DependsOn.ToList());
        }
    }

    /// <summary>
    /// Apply a topological sort to the graph, and return migrations that have / are dependencies in the sorted order.
    /// Note: see https://en.wikipedia.org/wiki/Topological_sorting for more info.`
    /// </summary>
    /// <returns>Migrations, in a reasonable order according to dependencies.</returns>
    private List<IMigrationBase> TopologicalSortMigrations() {
        List<string> topologicalOrdering = [];

        // Construct a mapping of migrations to their indegrees (number of edges into each version)
        Dictionary<string, int> indegrees = _allMigrations.ToDictionary(migration => migration.Version, _ => 0);
        foreach (string version in _nodes.Keys) {
            foreach (string dependency in _nodes[version]) {
                // Account for missing dependencies, likely due to migrations that are in the database already
                if (_allMigrations.Any(m => m.Version == dependency))
                    indegrees[version] += 1;
            }
        }

        // Start by adding versions with indegree 0
        // As long as there are versions with no incoming edges, keep adding them to the sorted collection
        Queue<string> versionsWithNoIncomingEdges = new(indegrees.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key));
        while (versionsWithNoIncomingEdges.Count > 0) {
            // Add one of those versions to the ordering
            string version = versionsWithNoIncomingEdges.Dequeue();
            topologicalOrdering.Add(version);

            IEnumerable<string> dependentVersions = _nodes
                .Where(kvp => kvp.Value.Contains(version))
                .Select(kvp => kvp.Key);

            // Decrement the indegree of that version's dependents
            foreach (string dependent in dependentVersions) {
                if (!indegrees.TryGetValue(dependent, out int value))
                    continue;

                value -= 1;
                if (value == 0)
                    versionsWithNoIncomingEdges.Enqueue(dependent);

                indegrees[dependent] = value;
            }
        }

        return _allMigrations
            .Where(m => topologicalOrdering.Contains(m.Version))
            .OrderBy(m => topologicalOrdering.IndexOf(m.Version))
            .ToList();
    }

    /// <summary>
    /// A recursive helper function used to find cycles for a particular node in the graph.
    /// </summary>
    /// <param name="node">Node / migration version.</param>
    /// <param name="visitedNodes">A collection of nodes that have already been visited.</param>
    /// <param name="recursionStack">A collection of nodes that are candidates for a cycle.</param>
    /// <returns>True if a cycle is found containing the provided node, else false.</returns>
    private bool IsCyclicRecursiveHelper(string node, List<string> visitedNodes, List<string> recursionStack) {
        if (recursionStack.Contains(node))
            return true;

        if (visitedNodes.Contains(node))
            return false;

        visitedNodes.Add(node);
        recursionStack.Add(node);
        List<string> dependencies = _nodes[node];

        foreach (string version in dependencies) {
            IMigrationBase dependency = _allMigrations.First(m => m.Version == version);
            if (
                dependency is IOrderedMigration orderedDependency &&
                IsCyclicRecursiveHelper(orderedDependency.Version, visitedNodes, recursionStack)
            )
                return true;
        }

        recursionStack.Remove(node);

        return false;
    }

    /// <summary>
    /// Determine if any cycles exist within the graph.
    /// </summary>
    /// <returns>True if the graph contains a cycle, else false.</returns>
    public bool IsCyclic() {
        List<string> visitedNodes = [];
        List<string> recursionStack = [];

        foreach (string version in _nodes.Keys)
            if (IsCyclicRecursiveHelper(version, visitedNodes, recursionStack))
                return true;

        return false;
    }

    /// <summary>
    /// Determine an order of nodes in the graph and return an ordered collection of migrations.
    /// </summary>
    /// <returns>An ordered collection of migrations.</returns>
    public List<IMigrationBase> GetOrderedMigrations() {
        if (IsCyclic())
            throw new Exception("Circular migration dependencies detected, unable to determine order");

        return TopologicalSortMigrations();
    }
}
