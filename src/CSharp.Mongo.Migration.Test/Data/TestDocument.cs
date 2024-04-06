using MongoDB.Bson;

namespace CSharp.Mongo.Migration.Test.Data;

public class TestDocumentV1 {
    public ObjectId Id { get; set; }
    public string? Name { get; set; }
    public int? Age { get; set; }
}

public class TestDocumentV2 {
    public ObjectId Id { get; set; }
    public string? FullName { get; set; }
    public int? Age { get; set; }
}

public class TestDocumentV3 {
    public ObjectId Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? Age { get; set; }
}
