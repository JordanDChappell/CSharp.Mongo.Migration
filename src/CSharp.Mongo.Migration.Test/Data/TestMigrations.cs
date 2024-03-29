using CSharp.Mongo.Migration.Interfaces;

using MongoDB.Driver;

namespace CSharp.Mongo.Migration.Test;

public class Migration1 : IMigration {
    public string Name => "Migration 1";
    public string Version => "1993.10.05 migration1";

    public Task DownAsync(IMongoDatabase database) {
        throw new NotImplementedException();
    }

    public Task UpAsync(IMongoDatabase database) {
        throw new NotImplementedException();
    }
}

public class Migration2 : IMigration {
    public string Name => "Migration 2";
    public string Version => "1993.10.07 migration2";

    public Task DownAsync(IMongoDatabase database) {
        throw new NotImplementedException();
    }

    public Task UpAsync(IMongoDatabase database) {
        throw new NotImplementedException();
    }
}

public class Migration3 : IMigration {
    public string Name => "Migration 3";
    public string Version => "1993.10.09 migration3";

    public Task DownAsync(IMongoDatabase database) {
        throw new NotImplementedException();
    }

    public Task UpAsync(IMongoDatabase database) {
        throw new NotImplementedException();
    }
}
