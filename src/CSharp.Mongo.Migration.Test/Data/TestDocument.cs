namespace CSharp.Mongo.Migration.Test;

public class TestDocumentV1 {
    public string? FullName { get; set; }
    public int? Age { get; set; }
}

public class TestDocumentV2 {
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? Age { get; set; }
}

public class TestDocumentV3 {
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
