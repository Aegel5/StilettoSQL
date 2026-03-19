# StilettoSQL
Ultra-minimalist driver-agnostic SQL library for C#

## Limitations
- Only async functions
- Only postion data parameters

## Profiles
Library uses a global profile for easily settings change.
```csharp
StGlobal.DefaultProfile = new StProfile { CreateConnection = () => ... }; // Set default profile
async Task Func() {
    StGlobal.ChangeProfileAsyncLocal(new StProfile { ... });
    // Now work with another settings in this async context
}
```

## Queries
All examples for PostgreSQL

`Query` for simple and static queries.
```csharp
await foreach (var rd in Query.ReadAllRows("SELECT * FROM table WHERE id=$1", 123)){
    var id = rd.Val<long>("id");
}
```
`QueryBuilder` for any complex or dynamic queries
```csharp
var deleted_cnt = await new QueryBuilder("DELETE from table")
    .WhereAndEq("val1", 10)
    .WhereAndEq("val2", 20)
    .ExecuteGetRowsTouched(); // SQL: DELETE from table WHERE (val1=$1) AND (val2=$2)
```
`Inserter` for basic insert queries
```csharp
var inserted_id = await new Inserter("table")
    .SkipOnConflict()
    .Value("id", 123)
    .ExecuteReturnField<long?>("id"); // SQL: INSERT into table(id) VALUES ($1) on conflict do nothing returning id
if (inserted_id != null) {
    Console.WriteLine($"Was inserted: {inserted_id}");
}
```

## Data conversion
`IStDataConverter` used for custom data conversions
## Transactions
`AutoTransaction` used for easy transactions
```csharp
{
    using var transaction = new AutoTransaction();
    // do as usual
    await transaction.CommitAsync();
}
```
