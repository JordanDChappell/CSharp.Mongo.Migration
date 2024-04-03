# CSharp Mongo Migrations

Simple script based MongoDB database migrations written and run in .NET.

`CSharp.Mongo.Migration` uses the [C# MongoDB Driver](https://www.mongodb.com/docs/drivers/csharp/current/) to update documents in script based single use migrations (rather than on-the-fly)

# Usage

## Installation

Nuget: https://www.nuget.org/packages/CSharp.Mongo.Migration

Install the using the Visual Studio package manager or using the dotnet CLI:

```
dotnet add package CSharp.Mongo.Migration
```

## Running Migrations

1. Create an instance of the `MigrationRunner` class

```csharp
MigrationRunner runner = new("yourDatabaseConnectionString");
```

2. Register an `IMigrationLocator` and run the migrations

```csharp
await runner
    .RegisterLocator(new AssemblyMigrationLocator("Assembly.With.Your.Migrations"))
    .RunAsync();
```

## Writing Migrations

A migration script should implement the `IMigration` interface. The following properties / methods will need to be defined:

- `string Name`
- `string Version`
- `Task UpAsync(IMongoDatabase database)`
- `Task DownAsync(IMongoDatabase database)`

An optional `bool Skip()` method can be defined that can be used to conditionally skip a migration.

`UpAsync` will be run when the `MigrationRunner.RunAsync` method is called.

`DownAsync` will be called when the `MigrationRunner.RestoreAsync(string)` method is called, if you don't expect to revert a migration, simply have this method throw an exception.

## Restoring Migrations

To revert or restore a migration run the `RestoreAsync("version")` function on an instance of the `MigrationRunner` class

```csharp
MigrationRunner runner = new("youDatabaseConnectionString");
await runner
    .RegisterLocator(new AssemblyMigrationLocator("Assembly.With.Migration.Version"))
    .RestoreAsync("version");
```

# Why Create Another Library?

There are a number of other libraries for MongoDB designed to be used in C#. This library was written out of necessity to be used in a large development team with fairly sizeable database instances. There are some use cases that this library caters to that others do not:

* Allow migrations to run out of order
* Provide a version identifier that is not a number or using SemVer
* Encourage `async` migration scripts

Using other libraries we found that migration versions could clash when using number or SemVer based identifiers, our team uses GitFlow to achieve the development throughput and organisation that we require and often larger feature branches would clash when selecting version numbers.

# Dependencies

This library uses the following packages:

- [C# MongoDB Driver](https://www.mongodb.com/docs/drivers/csharp/current/)

## Development Dependencies

We use the following libraries for testing / development purposes:

- [AutoMoqCore](https://www.nuget.org/packages/AutoMoqCore/)
- [EphemeralMongo7](https://www.nuget.org/packages/EphemeralMongo7)
- [xunit](https://www.nuget.org/packages/xunit)
