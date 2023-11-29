# Blazor.IndexedDB

An easy way to interact with IndexedDB and make it feel like EF Core but `async`.

## Version history
- 2.0.1:
    - Upgraded from `.NET Core 3.2.1` to `.NET Core 7.0.0`
    - Upgraded from `netstandard2.1` to `net8.0`
    - Changed namespace `IndexedDB.Blazor` to namespace `Blazor.IndexedDB`
    - Added `indexedDb.Blazor.js` to EmbeddedResource and removed `TG.Blazor.IndexedDB` from PackageReference
    - Added `IndexedDBManager.cs`
- 1.1.1:
    - Upgraded from `.NET Core 3.0.0-preview` to `.NET Core 3.2.1`
    - Upgraded form `netstandard2.0` to `netstandard2.1`
    - Upgraded form `C# 7.3` to `C# 8.0`
    - Upgraded `TG.Blazor.IndexedDB` from `0.9.0-beta` to `1.5.0-preview`
    - Changed `namespace Blazor.IndexedDB.Framework` to `namespace IndexedDB.Blazor`
    - Changed `private IndexedDBManager connector;` to `protected IndexedDBManager connector;` in `IndexedDb`
    - Changed `IndexedSet<T> : IEnumerable<T>` to `IndexedSet<T> : ICollection<T>`
    - Original code by [Jinjinov](https://github.com/Jinjinov)
    - Original repository at [IndexedDB.Blazor](https://github.com/Jinjinov/IndexedDB.Blazor)
- 1.0.1:
    - Original code by [Reshiru](https://github.com/Reshiru)
    - Original repository at [Blazor.IndexedDB.Framework](https://github.com/Reshiru/Blazor.IndexedDB.Framework)

## NuGet installation
The NuGet package is at: https://www.nuget.org/packages/Blazor.IndexedDB

Either install it from command line:
```powershell
PM> Install-Package Blazor.IndexedDB
```
Or include it in your project file:

    <PackageReference Include="Blazor.IndexedDB" Version="2.0.1" />

## Features
- Connect and create database
- Add record
- Remove record
- Edit record

## How to use
1. Add `Blazor.IndexedDB/indexedDb.Blazor.js` to your `index.html`
```html
<script src="_content/Blazor.IndexedDB/indexedDb.Blazor.js"></script>
```
2. Register `IndexedDbFactory` as a service.
```CSharp
services.AddSingleton<IIndexedDbFactory, IndexedDbFactory>();
```
- `IIndexedDbFactory` is used to create the database connection and will create the database instance for you.

- `IndexedDbFactory` requires an instance of `IJSRuntime` which should normally already be registered.

3. Create any code first database model and inherit from `IndexedDb`. Only properties with the type `IndexedSet<>` will be used, any other properties will be ignored.
```CSharp
public class ExampleDb : IndexedDb
{
  public ExampleDb(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version) { }
  public IndexedSet<Person> People { get; set; }
}
```
- Your model (eg. `Person`) should contain an `Id` property or a property marked with the `Key` attribute.
```CSharp
public class Person
{
  [System.ComponentModel.DataAnnotations.Key]
  public long Id { get; set; }
  public string FirstName { get; set; }
  public string LastName { get; set; }
}
```

4. Now you can start using your database.

- Usage in Razor via inject: `@inject IIndexedDbFactory DbFactory`

### Adding records
```CSharp
using (var db = await this.DbFactory.Create<ExampleDb>())
{
  db.People.Add(new Person()
  {
    FirstName = "First",
    LastName = "Last"
  });
  await db.SaveChanges();
}
```
### Removing records
To remove an element it is faster to use an already created reference. You should also be able to remove an object only by it's `Id` but you have to use the `.Remove(object)` method (eg. `.Remove(new object() { Id = 1 })`)
```CSharp
using (var db = await this.DbFactory.Create<ExampleDb>())
{
  var firstPerson = db.People.First();
  db.People.Remove(firstPerson);
  await db.SaveChanges();
}
```
### Modifying records
```CSharp
using (var db = await this.DbFactory.Create<ExampleDb>())
{
  var personWithId1 = db.People.Single(x => x.Id == 1);
  personWithId1.FirstName = "This is 100% a first name";
  await db.SaveChanges();
}
```

## License

Original [license](https://github.com/Reshiru/Blazor.IndexedDB.Framework/blob/master/LICENSE).

Licensed under the [MIT](LICENSE) license.
