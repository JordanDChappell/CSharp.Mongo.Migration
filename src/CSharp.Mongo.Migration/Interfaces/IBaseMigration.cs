namespace CSharp.Mongo.Migration.Interfaces;

public interface IBaseMigration {
    public string Name { get; }
    public string Version { get; }
}
