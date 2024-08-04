using CSharp.Mongo.Migration.Interfaces;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Test.Data;

public class TestMigration1 : IMigration {
    public string Name => "Migration 1";
    public string Version => "1993.10.05 migration1";

    public void Down(IMongoDatabase database) {
        var collection = database.GetCollection<TestDocumentV2>("Documents");

        var update = Builders<TestDocumentV2>.Update.Rename(d => d.FullName, nameof(TestDocumentV1.Name));

        collection.UpdateMany(_ => true, update);
    }

    public void Up(IMongoDatabase database) {
        var collection = database.GetCollection<TestDocumentV1>("Documents");

        var update = Builders<TestDocumentV1>.Update.Rename(d => d.Name, nameof(TestDocumentV2.FullName));

        collection.UpdateMany(_ => true, update);
    }
}

public class TestMigration2 : IAsyncMigration {
    public string Name => "Migration 2";
    public string Version => "1993.10.07 migration2";

    public Task DownAsync(IMongoDatabase database) {
        throw new NotImplementedException();
    }

    public async Task UpAsync(IMongoDatabase database) {
        var v2Collection = database.GetCollection<TestDocumentV2>("Documents");
        var v3Collection = database.GetCollection<TestDocumentV3>("Documents");
        var documents = await v2Collection.Find(_ => true).ToListAsync();

        foreach (var document in documents) {
            var nameSplit = document.FullName?.Split(" ");

            TestDocumentV3 newDocument = new() {
                Id = document.Id,
                FirstName = nameSplit?.FirstOrDefault(),
                LastName = nameSplit?.LastOrDefault(),
                Age = document.Age,
            };

            await v3Collection.ReplaceOneAsync(d => d.Id == newDocument.Id, newDocument);
        }
    }
}

public class TestMigration3 : IAsyncMigration {
    public string Name => "Migration 3";
    public string Version => "1993.10.09 migration3";

    public Task DownAsync(IMongoDatabase database) {
        throw new NotImplementedException();
    }

    public Task UpAsync(IMongoDatabase database) {
        return Task.CompletedTask;
    }
}

public class TestMigration4 : IAsyncMigration, IOrderedMigration {
    public string Name => "Migration 4";
    public string Version => "2024.01.01 migration4";
    public IEnumerable<string> DependsOn => new List<string>() {
        "1993.10.05 migration1"
    };

    public Task DownAsync(IMongoDatabase database) {
        throw new NotImplementedException();
    }

    public Task UpAsync(IMongoDatabase database) {
        return Task.CompletedTask;
    }
}

public class TestMigration5 : IAsyncMigration, IOrderedMigration {
    public string Name => "Migration 5";
    public string Version => "2024.02.01 migration5";
    public IEnumerable<string> DependsOn => new List<string>() {
        "2024.01.01 migration4"
    };

    public Task DownAsync(IMongoDatabase database) {
        throw new NotImplementedException();
    }

    public Task UpAsync(IMongoDatabase database) {
        return Task.CompletedTask;
    }
}

[IgnoreMigration]
public class TestCyclicMigration1 : IOrderedMigration {
    public IEnumerable<string> DependsOn => new List<string>() { "TestCyclicMigration2" };
    public string Name => "TestCyclicMigration1";
    public string Version => "TestCyclicMigration1";
}

[IgnoreMigration]
public class TestCyclicMigration2 : IOrderedMigration {
    public IEnumerable<string> DependsOn => new List<string>() { "TestCyclicMigration1" };
    public string Name => "TestCyclicMigration2";
    public string Version => "TestCyclicMigration2";
}
