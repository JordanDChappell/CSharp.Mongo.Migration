using CSharp.Mongo.Migration.Interfaces;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Test;

public class TestMigration1 : IMigration {
    public string Name => "Migration 1";
    public string Version => "1993.10.05 migration1";

    public async Task DownAsync(IMongoDatabase database) {
        var collection = database.GetCollection<TestDocumentV2>("Documents");

        var update = Builders<TestDocumentV2>.Update.Rename(d => d.FullName, nameof(TestDocumentV1.Name));

        await collection.UpdateManyAsync(_ => true, update);
    }

    public async Task UpAsync(IMongoDatabase database) {
        var collection = database.GetCollection<TestDocumentV1>("Documents");

        var update = Builders<TestDocumentV1>.Update.Rename(d => d.Name, nameof(TestDocumentV2.FullName));

        await collection.UpdateManyAsync(_ => true, update);
    }
}

public class TestMigration2 : IMigration {
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

public class TestMigration3 : IMigration {
    public string Name => "Migration 3";
    public string Version => "1993.10.09 migration3";

    public Task DownAsync(IMongoDatabase database) {
        throw new NotImplementedException();
    }

    public Task UpAsync(IMongoDatabase database) {
        throw new NotImplementedException();
    }
}
